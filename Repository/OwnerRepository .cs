using TodoWebApi.Data;
using TodoWebApi.Interface;
using TodoWebApi.Models;

namespace TodoWebApi.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        //region public 
        public OwnerRepository(DataContext context)
        {
            _context = context;
        }

        public Owner GetOwnerByEmail(string email)
        {
            return _context.Owners.Where(c => c.Email.Trim().ToUpper() == email.TrimEnd().ToUpper()).FirstOrDefault();
        }

        public ICollection<Owner> GetAllOwner()
        {
            return _context.Owners.ToList();
        }

        public ICollection<Tasks> GetTaskByOwner(string email)
        {
            return _context.Tasks.Where(t => t.Email.Trim().ToUpper() == email.TrimEnd().ToUpper()).ToList();
        }

        public bool OwnerExists(string email)
        {
            return _context.Owners.Any(o => o.Email.Trim().ToUpper() == email.TrimEnd().ToUpper());
        }

        public bool CreateOwner(Owner owner)
        {
            _context.Add(owner);
            return Save();
        }

        public bool UpdateOwner(Owner owner)
        {
            _context.Update(owner);
            return Save();
        }

        public bool DeleteOwner(Owner owner)
        {
            _context.Remove(owner);
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
