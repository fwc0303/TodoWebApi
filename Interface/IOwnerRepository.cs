using TodoWebApi.Dto;
using TodoWebApi.Models;

namespace TodoWebApi.Interface
{
    public interface IOwnerRepository
    {
        Owner GetOwnerByEmail(string email);
        ICollection<Owner> GetAllOwner();
        ICollection<Tasks> GetTaskByOwner(string email);
        bool OwnerExists(string email);
        bool CreateOwner(Owner owner);
        bool UpdateOwner(Owner owner);
        bool DeleteOwner(Owner owner);
        bool Save();
    }
}
