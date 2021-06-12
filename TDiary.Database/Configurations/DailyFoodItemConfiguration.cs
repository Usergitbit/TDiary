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
    public class DailyFoodItemConfiguration : BaseConfiguration<DailyFoodItem>
    {
        public override void Configure(EntityTypeBuilder<DailyFoodItem> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.Calories)
                .IsRequired();

            builder.Property(p => p.Carbohydrates)
                .IsRequired();

            builder.Property(p => p.Fats)
                .IsRequired();

            builder.Property(p => p.Proteins)
                .IsRequired();

            builder.Property(p => p.Quantity)
                .IsRequired();

            builder.Property(p => p.SaturatedFats)
                .IsRequired();

            builder.HasOne(dfi => dfi.FoodItem)
                .WithMany(fi => fi.DailyFoodItems)
                .HasForeignKey(dfi => dfi.FoodItemId)
                .IsRequired();

            builder.HasOne(dfi => dfi.Plate)
                .WithMany(p => p.DailyFoodItems)
                .HasForeignKey(dfi => dfi.PlateId);
        }
    }
}
