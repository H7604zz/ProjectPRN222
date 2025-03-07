using ProjectPrn222.Models;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Service.Implement
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }
        public IList<ApplicationUser> GetAllUsers()
        {
            return _context.Users.ToList();
        }
        public void AddUser(ApplicationUser user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public void EditUser(ApplicationUser user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
        public void DeleteUser(ApplicationUser user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
        public ApplicationUser? GetUserById(string userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }
        public bool IsEmailExisted(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

    }
}
