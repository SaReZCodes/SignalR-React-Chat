using System;
using System.Linq;
using System.Collections.Generic;
using SRRC.Common;

using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SRRC.DomainClasses.Entities.Authentication;
using SRRC.DataLayer.Database;

namespace SRRC.Service.Repository.Authentication
{
    public interface IRolesService
    {
        Task<List<Role>> FindUserRolesAsync(int userId);
        Task<bool> IsUserInRoleAsync(int userId, string roleName);
        Task<List<User>> FindUsersInRoleAsync(string roleName);
    }

    public class RolesService : IRolesService
    {
        private readonly SRRCDbContext _context;
        //private readonly IUnitOfWork _uow;
        private readonly DbSet<Role> _roles;
        private readonly DbSet<User> _users;

        public RolesService(SRRCDbContext context)
        {
            //int i = 0;
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));

            _roles = _context.Set<Role>();
            _users = _context.Set<User>();
        }

        public Task<List<Role>> FindUserRolesAsync(int userId)
        {
            var userRolesQuery = from role in _roles
                                 from userRoles in role.UserRoles
                                 where userRoles.UserId == userId
                                 select role;

            return userRolesQuery.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<bool> IsUserInRoleAsync(int userId, string roleName)
        {
            var userRolesQuery = from role in _roles
                                 where role.Name == roleName
                                 from user in role.UserRoles
                                 where user.UserId == userId
                                 select role;
            var userRole = await userRolesQuery.FirstOrDefaultAsync();
            return userRole != null;
        }

        public Task<List<User>> FindUsersInRoleAsync(string roleName)
        {
            var roleUserIdsQuery = from role in _roles
                                   where role.Name == roleName
                                   from user in role.UserRoles
                                   select user.UserId;
            return _users.Where(user => roleUserIdsQuery.Contains(user.Id))
                         .ToListAsync();
        }
    }
}