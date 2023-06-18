using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using TodoWebApi.Dto;
using TodoWebApi.Interface;
using TodoWebApi.Models;

namespace TodoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : Controller
    {   
        //region public 
        public TasksController(ITasksRepository tasksRepository,
                        IMemoryCache cache,
                        IMapper mapper)
        {
            _tasksRepository = tasksRepository;
            _cache = cache;
            _mapper = mapper;
        }

        [HttpGet("GetOwnTasks"), Authorize]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Tasks>))]
        public IActionResult GetOwnTasks()
        {
            var email = string.Empty;
            if (_cache.TryGetValue("UserName", out string emails))
            {
                email = emails;
            }

            var tasklists = _mapper.Map<List<Tasks>>(_tasksRepository.GetOwnTasks(email));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasklists);
        }

        [HttpGet("GetAllTasks"), Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Tasks>))]
        public IActionResult GetAllTasks()
        {
            var tasklists = _mapper.Map<List<Tasks>>(_tasksRepository.GetAllTasks());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasklists);
        }

        [HttpGet("GetTasksById/{taskId}")]
        [ProducesResponseType(200, Type = typeof(Tasks))]
        [ProducesResponseType(400)]
        public IActionResult GetTasksById(int taskId)
        {
            if (!_tasksRepository.TaskExists(taskId))
                return NotFound();

            var tasks = _mapper.Map<TasksDto>(_tasksRepository.GetTasksById(taskId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasks);
        }

        [HttpGet("GetAlert")]
        [ProducesResponseType(200, Type = typeof(Tasks))]
        [ProducesResponseType(400)]
        public IActionResult GetAlert()
        {
            var email = string.Empty;
            var message = string.Empty;
            if (_cache.TryGetValue("UserName", out string emails))
            {
                email = emails;
            }

            ushort priority = 1;
            ushort status = 3;

            var numberOfPriorityTasks = _tasksRepository.CheckTasksByPriority(email, priority);
            var numberOfExceedDueDay = _tasksRepository.CheckTasksExceedDueDate(email, status);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (numberOfExceedDueDay > 0)
            {
                message = "You have " + numberOfExceedDueDay + " incomplete tasks are expired!";
            }

            if (numberOfPriorityTasks > 0) 
            {
                message += "You still have " + numberOfPriorityTasks + " escalated task on hands!";
            } 

            if (message == string.Empty) 
            {
                message = "You have no escalated task on hands and no incomplete tasks expired yet.";
            }

            return Ok(message);
        }

        [HttpGet("SearchTasksByPriority/{priority}")]
        [ProducesResponseType(200, Type = typeof(Tasks))]
        [ProducesResponseType(400)]
        public IActionResult SearchTasksByPriority(ushort priority)
        {
            var email = string.Empty;
            if (_cache.TryGetValue("UserName", out string emails))
            {
                email = emails;
            }

            var tasks = _mapper.Map<List<TasksDto>>(_tasksRepository.GetTasksByPriority(email, priority));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasks);
        }

        [HttpGet("SearchTasksByStatus/{status}")]
        [ProducesResponseType(200, Type = typeof(Tasks))]
        [ProducesResponseType(400)]
        public IActionResult SearchTasksByStatus(ushort status)
        {
            var email = string.Empty;
            if (_cache.TryGetValue("UserName", out string emails))
            {
                email = emails;
            }

            var tasks = _mapper.Map<List<TasksDto>>(_tasksRepository.GetTasksByStatus(email, status));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasks);
        }


        [HttpGet("GetAllTaskSortedByDueDate")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Tasks>))]
        public IActionResult GetAllTaskSortByDueDate()
        {
            var email = string.Empty;
            if (_cache.TryGetValue("UserName", out string emails))
            {
                email = emails;
            }

            var tasklists = _mapper.Map<List<Tasks>>(_tasksRepository.GetAllTaskSortedByDueDate(email));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasklists);
        }

        [HttpGet("GetAllTaskSortedByPriority")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Tasks>))]
        public IActionResult GetAllTaskSortByPriority()
        {
            var email = string.Empty;
            if (_cache.TryGetValue("UserName", out string emails))
            {
                email = emails;
            }

            var tasklists = _mapper.Map<List<Tasks>>(_tasksRepository.GetAllTaskSortedByPriority(email));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasklists);
        }

        [HttpGet("GetAllTaskSortedByStatus")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Tasks>))]
        public IActionResult GetAllTaskSortedByStatus()
        {
            var email = string.Empty;
            if (_cache.TryGetValue("UserName", out string emails))
            {
                email = emails;
            }

            var tasklists = _mapper.Map<List<Tasks>>(_tasksRepository.GetAllTaskSortedByStatus(email));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasklists);
        }

        [HttpPost("createTask")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTask([FromBody] TasksDto request)
        {
            if (request == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = new Tasks();
            if (_cache.TryGetValue("UserName", out string emails)) 
            {
                task.Email = emails;
            }

            var enumPriorityValues = Enum.GetValues(typeof(PriorityEnum)).Cast<int>().ToArray();
            if (!enumPriorityValues.Contains(request.Priority)) 
            {
                ModelState.AddModelError("", "Priority not exists");
                return StatusCode(422, ModelState);
            }

            var enumStatusValues = Enum.GetValues(typeof(StatusEnum)).Cast<int>().ToArray();
            if (!enumStatusValues.Contains(request.Status))
            {
                ModelState.AddModelError("", "Status not exists");
                return StatusCode(423, ModelState);
            }

            task.Name = request.Name;
            task.Description = request.Description;
            task.DueDate = request.DueDate;
            task.Status = (StatusEnum)request.Status;
            task.Priority = (PriorityEnum)request.Priority;

            var taskMap = _mapper.Map<Tasks>(task);

            if (!_tasksRepository.CreateTask(taskMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("updateTaskById/{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateTask(int id,
            [FromBody] TasksDto request)
        {
            if (request == null)
                return BadRequest(ModelState);

            if (!_tasksRepository.TaskExists(id))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var task = new Tasks();
            if (_cache.TryGetValue("UserName", out string emails))
            {
                task.Email = emails;
            }

            var enumPriorityValues = Enum.GetValues(typeof(PriorityEnum)).Cast<int>().ToArray();
            if (!enumPriorityValues.Contains(request.Priority))
            {
                ModelState.AddModelError("", "Priority not exists");
                return StatusCode(422, ModelState);
            }

            var enumStatusValues = Enum.GetValues(typeof(StatusEnum)).Cast<int>().ToArray();
            if (!enumStatusValues.Contains(request.Status))
            {
                ModelState.AddModelError("", "Status not exists");
                return StatusCode(423, ModelState);
            }

            task.Id = id;
            task.Name = request.Name;
            task.Description = request.Description;
            task.DueDate = request.DueDate;
            task.Status = (StatusEnum)request.Status;
            task.Priority = (PriorityEnum)request.Priority;

            var taskMap = _mapper.Map<Tasks>(task);

            if (!_tasksRepository.UpdateTask(id, taskMap))
            {
                ModelState.AddModelError("", "Something went wrong updating owner");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("deleteTaskById/{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTask(int id)
        {
            if (!_tasksRepository.TaskExists(id))
            {
                return NotFound();
            }

            var taskToDelete = _tasksRepository.GetTasksById(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_tasksRepository.DeleteTask(taskToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner");
            }

            return Ok("Successfully updated");
        }
        //end region

        //region private 
        private readonly IMemoryCache _cache;
        private readonly ITasksRepository _tasksRepository;
        private readonly IMapper _mapper;
        //end region
    }
}
