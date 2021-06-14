using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDiary.Common.Models.Base;

namespace TDiary.Database.Configurations.Base
{
    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : EntityBase
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.InsertedAt)
                .IsRequired();

            builder.Property(p => p.LocallyCreatedAt)
                 .IsRequired();

            builder.Property(p => p.LocallyModifiedAt);

            builder.Property(p => p.TimeZone)
                 .HasConversion(tz => tz.ToSerializedString(), s => TimeZoneInfo.FromSerializedString(s))
                 .IsRequired();

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.Property(p => p.CreatedBy)
                .IsRequired();
        }
    }
}
