using ProjectPrn222.Models;

namespace ProjectPrn222.Service.Iterface
{
    public interface IAuthService
    {
        ApplicationUser? AuthenticateUserAsync(string username, string password);
    }
}
