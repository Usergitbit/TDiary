using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Database.Configurations.Base;

namespace TDiary.Database.Configurations
{
    public class EventConfiguration : BaseConfiguration<Event>
    {
        public override void Configure(EntityTypeBuilder<Event> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.Data)
                .IsRequired();

            builder.Property(p => p.Entity)
                .IsRequired();

            builder.Property(p => p.EventType)
                .IsRequired();

            builder.Property(p => p.Version)
                .IsRequired();
        }
    }
}
