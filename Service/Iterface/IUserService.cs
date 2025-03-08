using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;

namespace ProjectPrn222.Service.Iterface
{
    public interface IUserService
    {
        IQueryable<UserViewModel> GetAllUsers();
        void AddUser(ApplicationUser user);
        void EditUser(ApplicationUser user);
        void DeleteUser(ApplicationUser user);
        ApplicationUser? GetUserById(string userId);
        bool IsEmailExisted(string email);
        IQueryable<UserViewModel>? SearchUser(string keyword);
    }
}
