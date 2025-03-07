using Microsoft.EntityFrameworkCore;
using ProjectPrn222.Models;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Service.Implement
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;


        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public ApplicationUser? AuthenticateUserAsync(string un, string pw)
        {
            var adminUsername = _configuration["AdminAccount:Username"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (un == adminUsername && pw == adminPassword)
            {
                return new ApplicationUser
                {
                    Id = "",
                };
            }

            var user = _context.Users.FirstOrDefault(u => u.UserName == un && u.PasswordHash == pw);

            if (user == null)
                return null;

            return user;
        }
    }
}
