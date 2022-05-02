using System;
using System.Security.Claims;
using System.Threading.Tasks;
using SRRC.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SRRC.DataLayer.Database;
using SRRC.DomainClasses.Entities.Authentication;
using System.Collections.Generic;
using System.Linq;

namespace SRRC.Service.Repository.Authentication
{
    public interface IUsersService
    {
        Task<string> GetSerialNumberAsync(int userId);
        Task<User> FindUserAsync(string username, string password);
        ValueTask<User> FindUserAsync(int userId);
        Task UpdateUserLastActivityDateAsync(int userId);
        ValueTask<User> GetCurrentUserAsync();
        Task<(bool Succeeded, string Error)> Register(User user);
        Task<(bool Succeeded, string Error)> RegisterAdmin(User user);
        int GetCurrentUserId();
        Task<(bool Succeeded, string Error)> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<(bool Succeeded, string Error)> ChangePasswordAdminAsync(User user, string newPassword);
        Task<List<User>> GetUseSRRCsync();
        void Update(User user);
        void Remove(User user);
        void AddAdminUserRole(int userId);
        void DeleteAdminUserRole(int userId);
        bool IsAdminUser(int userId);
    }

    public class UsersService : IUsersService
    {
        private readonly SRRCDbContext _context;
        private readonly DbSet<User> _users;
        private readonly ISecurityService _securityService;
        private readonly IHttpContextAccessor _contextAccessor;


        public UsersService(
            SRRCDbContext context,
            ISecurityService securityService,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));

            _users = _context.Set<User>();

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(_securityService));

            _contextAccessor = contextAccessor;
            _contextAccessor.CheckArgumentIsNull(nameof(_contextAccessor));
        }

        public void AddAdminUserRole(int userId)
        {
            try
            {
                var userRole = new UserRole()
                {
                    UserId = userId,
                    RoleId = 1
                };
                _context.UserRoles.Add(userRole);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteAdminUserRole(int userId)
        {
            var userRoles = _context.UserRoles.Where(x => x.RoleId == 1 && x.UserId == userId).ToList();
            userRoles.ForEach(x =>
            {
                _context.UserRoles.Remove(x);
            });
        }

        public bool IsAdminUser(int userId)
        {
            return _context.UserRoles.Where(x => x.UserId == userId && x.RoleId == 1).Any();
        }

        public ValueTask<User> FindUserAsync(int userId)
        {
            return _users.FindAsync(userId);
        }

        public Task<User> FindUserAsync(string username, string password)
        {
            var passwordHash = _securityService.GetSha256Hash(password);
            return _users.FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower() && x.Password == passwordHash);
        }

        public async Task<string> GetSerialNumberAsync(int userId)
        {
            var user = await FindUserAsync(userId);
            return user.SerialNumber;
        }

        public async Task UpdateUserLastActivityDateAsync(int userId)
        {
            var user = await FindUserAsync(userId);
            if (user.LastLoggedIn != null)
            {
                var updateLastActivityDate = TimeSpan.FromMinutes(2);
                var currentUtc = DateTimeOffset.UtcNow;
                var timeElapsed = currentUtc.Subtract(user.LastLoggedIn.Value);
                if (timeElapsed < updateLastActivityDate)
                {
                    return;
                }
            }

            user.LastLoggedIn = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
        }

        public int GetCurrentUserId()
        {
            var claimsIdentity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userDataClaim = claimsIdentity?.FindFirst(ClaimTypes.UserData);
            var userId = userDataClaim?.Value;
            return string.IsNullOrWhiteSpace(userId) ? 0 : int.Parse(userId);
        }

        public ValueTask<User> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            return FindUserAsync(userId);
        }

        public async Task<(bool Succeeded, string Error)> ChangePasswordAsync(User user, string currentPassword,
            string newPassword)
        {
            var currentPasswordHash = _securityService.GetSha256Hash(currentPassword);
            if (user.Password != currentPasswordHash)
            {
                return (false, "رمز عبور درست نمیباشد.");
            }

            user.SerialNumber = Guid.NewGuid().ToString("N");
            user.Password = _securityService.GetSha256Hash(newPassword);
            // To force other logins to expire.
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool Succeeded, string Error)> ChangePasswordAdminAsync(User user, string newPassword)
        {
            user.SerialNumber = Guid.NewGuid().ToString("N");
            user.Password = _securityService.GetSha256Hash(newPassword);
            // To force other logins to expire.
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }


        public async Task<(bool Succeeded, string Error)> Register(User user)
        {
            // var currentPasswordHash = _securityService.GetSha256Hash(currentPassword);
            // if (user.Password != currentPasswordHash)
            // {
            //     return (false, "Current password is wrong.");
            // }
            user.IsActive = true;
            user.Password = _securityService.GetSha256Hash(user.Password);
            _context.Users.Add(user);
            _context.Add(new UserRole {Role = _context.Roles.Find(2), User = user});
            // user.SerialNumber = Guid.NewGuid().ToString("N"); // To force other logins to expire.
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool Succeeded, string Error)> RegisterAdmin(User user)
        {
            // var currentPasswordHash = _securityService.GetSha256Hash(currentPassword);
            // if (user.Password != currentPasswordHash)
            // {
            //     return (false, "Current password is wrong.");
            // }
            user.Password = _securityService.GetSha256Hash(user.Password);
            _context.Users.Add(user);
            _context.Add(new UserRole {Role = _context.Roles.Find(3), User = user});

            // user.SerialNumber = Guid.NewGuid().ToString("N"); // To force other logins to expire.
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<List<User>> GetUseSRRCsync()
        {
            return await _context.Users.ToListAsync();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Remove(User user)
        {
            _context.Remove(user);
        }
    }
}