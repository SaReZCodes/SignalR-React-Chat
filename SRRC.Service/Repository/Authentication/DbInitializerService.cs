using System;
using System.Linq;

using SRRC.Common;
using Microsoft.Extensions.DependencyInjection;
using SRRC.DataLayer.Database;
using SRRC.DomainClasses.Entities.Authentication;

namespace SRRC.Service.Repository.Authentication
{
    public interface IDbInitializerService
    {
        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Adds some default values to the Db
        /// </summary>
        void SeedData();
    }

    public class DbInitializerService : IDbInitializerService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISecurityService _securityService;

        public DbInitializerService(
            IServiceScopeFactory scopeFactory,
            ISecurityService securityService)
        {
            _scopeFactory = scopeFactory;
            _scopeFactory.CheckArgumentIsNull(nameof(_scopeFactory));

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(_securityService));
        }

        public void Initialize()
        {
            //using (var serviceScope = _scopeFactory.CreateScope())
            //{
            //    using (var context = serviceScope.ServiceProvider.GetService<SRRCDbContext>())
            //    {
            //        context.Database.Migrate();
            //    }
            //}
        }

        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<SRRCDbContext>())
                {
                    // Add default roles
                    var adminRole = new Role { Name = CustomRoles.Admin };
                    var userRole = new Role { Name = CustomRoles.User };
                    var employeeRole = new Role { Name = CustomRoles.Employee };
                    if (!context.Roles.Any())
                    {
                        context.Add(adminRole);
                        context.Add(userRole);
                        context.Add(employeeRole);
                        context.SaveChanges();
                    }
                    // Add Admin user
                    if (!context.Users.Any())
                    {
                        var adminUser = new User
                        {
                            Username = "Admin",
                            DisplayName = "مدیر سیستم",
                            IsActive = true,
                            LastLoggedIn = null,
                            Password = _securityService.GetSha256Hash("123456"),
                            SerialNumber = Guid.NewGuid().ToString("N")
                        };
                        context.Add(adminUser);
                        context.SaveChanges();

                        context.Add(new UserRole { Role = adminRole, User = adminUser });
                        context.Add(new UserRole { Role = employeeRole, User = adminUser });

                        context.SaveChanges();
                    }
                    // Add default SubCriterion
                    // if (!context.SubCriterions.Any())
                    // {
                    //     var defaultSubCriterion = new SubCriterion
                    //     {
                    //         Id = 1,
                    //         Title = "عمومی",
                    //         MaxPoint = 0
                    //     };
                    //     context.Add(defaultSubCriterion);
                    //     context.SaveChanges();
                    // }
                }
            }
        }
    }
}