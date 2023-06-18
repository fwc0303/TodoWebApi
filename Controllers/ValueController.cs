using Microsoft.AspNetCore.Mvc;

namespace TodoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValueController : ControllerBase
    {
        [HttpGet]
        public string GetData() 
        {
            return "Testing";
        }
    }
}
