using EmployeeManagement.ApplicationContext;
using EmployeeManagement.Model;
using EmployeeManagement.Service.Interfaces;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.EntityFrameworkCore;
using RestAPI.Exceptions;
using EmployeeManagement.RequestModal;

namespace EmployeeManagement.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterAsync(AdminRm req)
        {
            bool emailExists = await _context.Admins
                .AnyAsync(x => x.Email == req.Email && !x.IsDeleted);

            if (emailExists)
            {
                throw new AppExceptions("Email already registered.",409);
            }

            var admin = new Admin
            {
                FullName = req.FullName,
                Email = req.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
            };
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LoginRes> LoginAsync(LoginReq req)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(x =>
                    x.Email == req.Email &&
                    !x.IsDeleted);

            if (admin == null)
            {
                throw new AppExceptions(
                    "Invalid email or password.",
                    401);
            }

            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(
                    req.Password,
                    admin.Password);

            if (!isPasswordValid)
            {
                throw new AppExceptions(
                    "Invalid email or password.",
                    401);
            }

            return new LoginRes
            {
                FullName = admin.FullName,
                Email = admin.Email,
                Token = string.Empty
            };
        }
    }
}