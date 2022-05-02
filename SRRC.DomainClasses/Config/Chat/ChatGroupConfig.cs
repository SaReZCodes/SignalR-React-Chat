using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SRRC.DomainClasses.Entities.Authentication;
using SRRC.DomainClasses.Config;
using SRRC.DomainClasses.Entities.Chat;

namespace SRRC.DomainClasses.Config.Chat
{
    public class ChatGroupConfig : BaseEntityTypeConfiguration<ChatGroup>
    {
        public override void Configure(EntityTypeBuilder<ChatGroup> builder)
        {
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            builder.HasIndex(e => e.Title).IsUnique();

            builder.HasOne(x => x.Owner)
           .WithMany(x => x.ChatGroups).OnDelete(DeleteBehavior.Restrict);
            base.Configure(builder);
        }
    }
}
