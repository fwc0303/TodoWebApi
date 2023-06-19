using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using TodoWebApi.Controllers;
using TodoWebApi.Dto;
using TodoWebApi.Interface;
using TodoWebApi.Models;
using TodoWebApi.Service;
using Xunit;

namespace TodoWebApi.Tests
{
    public class OwnerControllerTest
    {
        //region public
        public OwnerControllerTest()
        {
            _ownerRepositoryMock = new Mock<IOwnerRepository>();
            _ownerServiceMock = new Mock<IOwnerService>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _configurationMock = new Mock<IConfiguration>();
            _mapperMock = new Mock<IMapper>();

            _controller = new OwnerController(
                _ownerRepositoryMock.Object,
                _ownerServiceMock.Object,
                _configurationMock.Object,
                _memoryCacheMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public void CreateOwner_ReturnsOwnerAlreadyExists()
        {
            var request = new OwnerDto
            {
                Email = "test@demo.com",
                Password = "password",
                Name = "Mr Bean",
                Role = "Admin"
            };

            _ownerRepositoryMock.Setup(repo => repo.GetOwnerByEmail(request.Email))
                .Returns(new Owner());

            // Act
            var result = _controller.CreateOwner(request) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(422, result.StatusCode);
        }


        [Fact]
        public void CreateOwner_ReturnsOkResult()
        {
            // Arrange
            var request = new OwnerDto
            {
                Email = "angie@demo.com",
                Password = "password",
                Name = "Ms Angie",
                Role = "Engineer"
            };

            _ownerRepositoryMock.Setup(repo => repo.GetOwnerByEmail(request.Email))
                .Returns((Owner)null);

            _ownerRepositoryMock.Setup(repo => repo.CreateOwner(It.IsAny<Owner>()))
                .Returns(true);

            // Act
            var result = _controller.CreateOwner(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Successfully created", result.Value);
            Assert.Equal(200, result.StatusCode);
        }
        //end region

        //private region
        private readonly Mock<IOwnerRepository> _ownerRepositoryMock;
        private readonly Mock<IOwnerService> _ownerServiceMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OwnerController _controller;
        //end region
    }
}
