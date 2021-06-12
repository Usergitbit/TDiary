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
    public class PlateConfiguration : BaseConfiguration<Plate>
    {
        public override void Configure(EntityTypeBuilder<Plate> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.Name)
                .IsRequired();

            builder.Property(p => p.Weight)
                .IsRequired();

            builder.HasMany(p => p.DailyFoodItems)
                .WithOne(dfi => dfi.Plate)
                .HasForeignKey(dfi => dfi.PlateId);
        }
    }
}
