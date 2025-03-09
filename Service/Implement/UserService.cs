using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;
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
        public IQueryable<UserViewModel> GetAllUsers()
        {
            return from u in _context.Users
                   join ur in _context.UserRoles on u.Id equals ur.UserId
                   join r in _context.Roles on ur.RoleId equals r.Id
                   select new UserViewModel
                  {
                      UserId = u.Id,
                      UserName = u.UserName,
                      Password = u.PasswordHash,
                      ConfirmPassword = u.PasswordHash,
                      Email = u.Email,
                      RoleName = r.Name
                  };
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

        public IQueryable<UserViewModel>? SearchUser(string keyword)
        {
            return from u in _context.Users
                   join ur in _context.UserRoles on u.Id equals ur.UserId
                   join r in _context.Roles on ur.RoleId equals r.Id
                   where u.Email.Contains(keyword) || u.UserName.Contains(keyword)
                   select new UserViewModel
                   {
                       UserId = u.Id,
                       UserName = u.UserName,
                       Password = u.PasswordHash,
                       ConfirmPassword = u.PasswordHash,
                       Email = u.Email,
                       RoleName = r.Name
                   };
        }
    }
}
