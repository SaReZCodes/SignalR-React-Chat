using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace SRRC.DomainClasses.Config
{
    public abstract class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T>
        where T : class
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property<bool>("IsDeleted")
                .IsRequired()
                .HasDefaultValue(false);
            builder.Property<DateTime>("InsertDateTime")
                .IsRequired()
                .HasDefaultValueSql("DateTime('now')")//"DateTime('now')" -> sqlite  //SYSDATETIME() -> sql server
                .ValueGeneratedOnAdd();
            builder.Property<DateTime>("UpdateDateTime")
                .IsRequired()
                .HasDefaultValueSql("DateTime('now')")
                .ValueGeneratedOnAddOrUpdate();
            builder.Property<string>("IpAddressLog");
            builder.Property<int?>("UserIdLog");
            builder.Property<DateTime?>("ModifiedDateLog");
        }
    }
}
