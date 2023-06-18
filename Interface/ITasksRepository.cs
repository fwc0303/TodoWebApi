using TodoWebApi.Models;

namespace TodoWebApi.Interface
{
    public interface ITasksRepository
    {
        Tasks GetTasksById(int id);
        ICollection<Tasks> GetOwnTasks(string email);
        ICollection<Tasks> GetAllTasks();
        ICollection<Tasks> GetTasksByDueDate(string email, DateTime dueDate);
        ICollection<Tasks> GetTasksByPriority(string email, ushort priority);
        ICollection<Tasks> GetTasksByStatus(string email, ushort status);
        ICollection<Tasks> GetAllTaskSortedByDueDate(string email);
        ICollection<Tasks> GetAllTaskSortedByPriority(string email);
        ICollection<Tasks> GetAllTaskSortedByStatus(string email);
        int CheckTasksExceedDueDate(string email, ushort status);
        int CheckTasksByPriority(string email, ushort priority);
        bool TaskExists(int id);
        bool CreateTask(Tasks task);
        bool UpdateTask(int id, Tasks task);
        bool DeleteTask(Tasks task);
        bool Save();
    }
}
