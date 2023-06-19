using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using TodoWebApi.Controllers;
using TodoWebApi.Dto;
using TodoWebApi.Interface;
using Xunit;

namespace TodoWebApi.Tests
{
    public class TasksControllerTest
    {
        //region public
        public TasksControllerTest()
        {
            _tasksRepositoryMock = new Mock<ITasksRepository>();
            _cacheMock = new Mock<IMemoryCache>();
            _mapperMock = new Mock<IMapper>();

            _controller = new TasksController(
                _tasksRepositoryMock.Object,
                _cacheMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public void CreateTask_StatusNotExists()
        {
            var request = new TasksDto
            {
                Name = "Development",
                Description = "Description",
                DueDate = DateTime.Now,
                Status = 10,
                Priority = 1
            };

            // Act
            var result = _controller.CreateTask(request) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(423, result.StatusCode);
        }

        [Fact]
        public void CreateTask_PriorityNotExists()
        {
            var request = new TasksDto
            {
                Name = "Development",
                Description = "Description",
                DueDate = DateTime.Now,
                Status = 1,
                Priority = 8
            };

            // Act
            var result = _controller.CreateTask(request) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(422, result.StatusCode);
        }

        [Fact]
        public void CreateTask_SomethingWentWrong()
        {
            var request = new TasksDto
            {
                Name = "Development",
                Description = "Description",
                DueDate = DateTime.Now,
                Status = 1,
                Priority = 2
            };

            // Act
            var result = _controller.CreateTask(request) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }

        //end region

        //private region
        private readonly Mock<ITasksRepository> _tasksRepositoryMock;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TasksController _controller;
        //end region
    }
}
