using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SRRC.DomainClasses.Config;
using SRRC.DomainClasses.Entities.Authentication;

namespace SRRC.DomainClasses.Config.Authentication
{
    public class GroupConfig : BaseEntityTypeConfiguration<Group>
    {
        public override void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.Property(e => e.Title).HasMaxLength(50).IsRequired();
            builder.HasIndex(e => e.Title).IsUnique();
            base.Configure(builder);
        }
    }
}
