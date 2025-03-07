using ProjectPrn222.Models;

namespace ProjectPrn222.Service.Iterface
{
    public interface IUserService
    {
        IList<ApplicationUser> GetAllUsers();
        void AddUser(ApplicationUser user);
        void EditUser(ApplicationUser user);
        void DeleteUser(ApplicationUser user);
        ApplicationUser? GetUserById(string userId);
        bool IsEmailExisted(string email);
    }
}
