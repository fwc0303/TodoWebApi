using TodoWebApi.Data;
using TodoWebApi.Interface;
using TodoWebApi.Models;

namespace TodoWebApi.Repository
{
    public class TasksRepository : ITasksRepository
    {
        //region public 
        public TasksRepository(DataContext context)
        {
            _context = context;
        }

        public Tasks GetTasksById(int id)
        {
            return _context.Tasks.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Tasks> GetOwnTasks(string email)
        {
            return _context.Tasks.Where(p => p.Email == email).OrderBy(p => p.Id).ToList();
        }

        public ICollection<Tasks> GetAllTasks()
        {
            return _context.Tasks.OrderBy(p => p.Id).ToList();
        }

        public ICollection<Tasks> GetTasksByDueDate(string email, DateTime dueDate)
        {
            return _context.Tasks.Where(p => (p.DueDate == dueDate)).Where(p => p.Email == email).ToList();
        }

        public ICollection<Tasks> GetTasksByPriority(string email, ushort priority)
        {
            return _context.Tasks.Where(p => ((ushort)p.Priority) == priority).Where(p => p.Email == email).ToList();
        }

        public ICollection<Tasks> GetTasksByStatus(string email, ushort status)
        {
            return _context.Tasks.Where(p => ((ushort)p.Status) == status).Where(p => p.Email == email).ToList();
        }

        public ICollection<Tasks> GetAllTaskSortedByDueDate(string email)
        {
            return _context.Tasks.Where(p => p.Email == email).OrderBy(p => p.DueDate).ToList();
        }

        public ICollection<Tasks> GetAllTaskSortedByPriority(string email)
        {
            return _context.Tasks.Where(p => p.Email == email).OrderBy(p => ((ushort)p.Priority)).ToList();
        }

        public ICollection<Tasks> GetAllTaskSortedByStatus(string email)
        {
            return _context.Tasks.Where(p => p.Email == email).OrderBy(p => ((ushort)p.Status)).ToList();
        }

        public int CheckTasksExceedDueDate(string email, ushort status)
        {
            return _context.Tasks.Where(p => ((ushort)p.Status) != status).Where(p => (p.DueDate < System.DateTime.Today)).Where(p => p.Email == email).Count();
        }

        public int CheckTasksByPriority(string email, ushort priority)
        {
            return _context.Tasks.Where(p => ((ushort)p.Priority) == priority).Where(p => (p.DueDate >= System.DateTime.Today)).Where(p => p.Email == email).Count();
        }

        public bool TaskExists(int id)
        {
            return _context.Tasks.Any(p => p.Id == id);
        }

        public bool CreateTask(Tasks task)
        {
            _context.Add(task);
            return Save();
        }

        public bool UpdateTask(int id, Tasks task)
        {
            _context.Update(task);
            return Save();
        }

        public bool DeleteTask(Tasks task)
        {
            _context.Remove(task);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        //end region

        //region private 
        private readonly DataContext _context;
        //end region
    }
}
