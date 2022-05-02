using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SRRC.DomainClasses.Config.Authentication;
using SRRC.DomainClasses.Config.Chat;
using SRRC.DomainClasses.Entities.Authentication;
using SRRC.DomainClasses.Entities.Chat;

namespace SRRC.DataLayer.Database
{
    public sealed class SRRCDbContext : DbContext
    {
        public readonly IHttpContextAccessor _accessor;
        public SRRCDbContext(DbContextOptions options, IHttpContextAccessor accessor) : base(options)
        {
            _accessor = accessor;
        }

        #region DbSets
        #region Authentication
        public DbSet<User>? Users { set; get; }
        public DbSet<Role>? Roles { set; get; }
        public DbSet<UserRole>? UserRoles { get; set; }
        public DbSet<Group>? Groups { set; get; }
        public DbSet<UserGroup>? UserGroups { get; set; }
        public DbSet<UserToken>? UserTokens { get; set; }
        #endregion
        public DbSet<ChatGroup> ChatGroup { get; set; }
        public DbSet<Chat> Chat { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Configurations
            //auth Configs
            builder.ApplyConfiguration(new RoleConfig());
            builder.ApplyConfiguration(new UserConfig());
            builder.ApplyConfiguration(new UserRoleConfig());
            builder.ApplyConfiguration(new UserTokenConfig());
            builder.ApplyConfiguration(new GroupConfig());
            builder.ApplyConfiguration(new UserGroupConfig());
            builder.ApplyConfiguration(new GroupRoleConfig());
            builder.ApplyConfiguration(new ChatGroupConfig());
            builder.ApplyConfiguration(new ChatConfig());
            #endregion
            builder.Seed();
        }
    }
}