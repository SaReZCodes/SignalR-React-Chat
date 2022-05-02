using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SRRC.DomainClasses.Entities.Authentication;
using SRRC.DomainClasses.Config;

namespace SRRC.DomainClasses.Config.Authentication
{
    public class RoleConfig : BaseEntityTypeConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(e => e.Name).HasMaxLength(450).IsRequired();
            builder.HasIndex(e => e.Name).IsUnique();
            base.Configure(builder);
        }
    }
}
