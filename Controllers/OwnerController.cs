using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TodoWebApi.Dto;
using TodoWebApi.Interface;
using TodoWebApi.Models;
using TodoWebApi.Service;

namespace TodoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        //region public 
        public static Owner owner = new Owner();
        public OwnerController(IOwnerRepository ownerRepository,
                        IOwnerService ownerService,
                        IConfiguration configuration,
                        IMemoryCache cache,
                        IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _cache = cache;
            _configuration = configuration;
            _ownerService = ownerService;
            _mapper = mapper;
        }

        [HttpGet("MyName"), Authorize(Roles = "Admin")]
        public ActionResult<string> GetMe()
        {
            var userName = _ownerService.GetMyName();
            return Ok(userName);
        }

        [HttpGet("MyEmail"), Authorize]
        public ActionResult<string> GetEmail()
        {
            var email = _ownerService.GetMyEmail();
            return Ok(email);
        }

        [HttpPost("register")]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromBody] OwnerDto request)
        {
            if (request == null)
                return BadRequest(ModelState);

            var owners = _ownerRepository.GetOwnerByEmail(request.Email);

            if (owners != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            owner.Email = request.Email;
            owner.PasswordHash = passwordHash;
            owner.PasswordSalt = passwordSalt;
            owner.Name = request.Name;
            owner.Role = request.Role;

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);

            var ownerMap = _mapper.Map<Owner>(owner);

            if (!_ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            var owners = _ownerRepository.GetOwnerByEmail(request.Email);

            if (owners == null)
            {
                ModelState.AddModelError("", "Owner not exists");
                return StatusCode(422, ModelState);
            }

            if (!VerifyPasswordHash(request.Password, owners.PasswordHash, owners.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            _cache.Set("UserName", request.Email);
            owner.Name = owners.Name;
            owner.Email = owners.Email;
            owner.Role = owners.Role;

            string token = CreateToken();

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);

            return Ok(token);
        }

        [HttpGet("GetAllOwner"), Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetAllOwner()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetAllOwner());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<string> nameList = new List<string> { };

            foreach (var owner in owners)
            {
                nameList.Add(owner.Name);
            }

            return Ok(nameList);
        }

        [HttpGet("GetOwnerByEmail/{email}"), Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnerByEmail(string email)
        {
            if (!_ownerRepository.OwnerExists(email))
            {
                return NotFound();
            }

            var owner = _mapper.Map<Owner>(
                _ownerRepository.GetOwnerByEmail(email));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner.Name);
        }

        //after login only can check
        [HttpGet("GetTaskByOwner/{email}"), Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(Tasks))]
        [ProducesResponseType(400)]
        public IActionResult GetTaskByOwner(string email)
        {
            if (!_ownerRepository.OwnerExists(email))
            {
                return NotFound();
            }

            var owner = _mapper.Map<List<Tasks>>(
                _ownerRepository.GetTaskByOwner(email));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner);
        }

        [HttpPut("UpdateOwner"), Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner([FromBody] OwnerUpdateDto request)
        {
            if (request == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_ownerRepository.OwnerExists(owner.Email))
            {
                return NotFound();
            }

            var owners = _ownerRepository.GetOwnerByEmail(owner.Email);

            if (owners == null)
            {
                ModelState.AddModelError("", "Owner not exists");
                return StatusCode(422, ModelState);
            }

            if (!VerifyPasswordHash(request.OldPassword, owners.PasswordHash, owners.PasswordSalt))
            {
                return BadRequest("Old password is wrong.");
            }

            if (!request.NewPassword.Equals(request.ConfirmPassword)) 
            {
                return BadRequest("New password is mistmatch with ConfirmPassword.");
            }

            CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            owners.PasswordHash = passwordHash;
            owners.PasswordSalt = passwordSalt;

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);

            var ownerMap = _mapper.Map<Owner>(owners);

            if (!_ownerRepository.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner");
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("DeleteOwner/{email}"), Authorize(Roles = "Admin")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(string email)
        {
            if (!_ownerRepository.OwnerExists(email))
            {
                return NotFound();
            }

            var ownerToDelete = _ownerRepository.GetOwnerByEmail(email);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_ownerRepository.DeleteOwner(ownerToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner");
            }

            return Ok("Successfully updated");
        }
        //end region

        //region private
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken()
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, owner.Name),
                new Claim(ClaimTypes.Email, owner.Email),
                new Claim(ClaimTypes.Role, owner.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            owner.RefreshToken = newRefreshToken.Token;
            owner.TokenCreated = newRefreshToken.Created;
            owner.TokenExpires = newRefreshToken.Expires;
        }

        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IOwnerService _ownerService;
        private readonly IMapper _mapper;
        //end region
    }
}
