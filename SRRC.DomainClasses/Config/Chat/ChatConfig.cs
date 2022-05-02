using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SRRC.DomainClasses.Config.Chat
{
    public class ChatConfig : BaseEntityTypeConfiguration<SRRC.DomainClasses.Entities.Chat.Chat>
    {
        public override void Configure(EntityTypeBuilder<SRRC.DomainClasses.Entities.Chat.Chat> builder)
        {
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");

            builder.HasOne(x => x.ChatGroup)
                .WithMany(x => x.Chats).OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
