using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SRRC.DomainClasses.Entities.Authentication;
using SRRC.DomainClasses.Config;

namespace SRRC.DomainClasses.Config.Authentication
{
    public class GroupRoleConfig : BaseEntityTypeConfiguration<GroupRole>
    {
        public override void Configure(EntityTypeBuilder<GroupRole> builder)
        {
            builder.HasKey(e => new { e.GroupId, e.RoleId });
            builder.HasIndex(e => e.GroupId);
            builder.HasIndex(e => e.RoleId);
            builder.Property(e => e.GroupId);
            builder.Property(e => e.RoleId);
            builder.HasOne(d => d.Role).WithMany(p => p.GroupRoles).HasForeignKey(d => d.RoleId);
            builder.HasOne(d => d.User).WithMany(p => p.GroupRoles).HasForeignKey(d => d.GroupId);
            base.Configure(builder);
        }
    }
}
