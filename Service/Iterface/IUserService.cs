using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;

namespace ProjectPrn222.Service.Iterface
{
    public interface IUserService
    {
        IQueryable<UserViewModel> GetAllUsers();
        IQueryable<UserViewModel>? SearchUser(string keyword);
    }
}
