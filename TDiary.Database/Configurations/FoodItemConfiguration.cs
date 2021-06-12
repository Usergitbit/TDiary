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
    public class FoodItemConfiguration : BaseConfiguration<FoodItem>
    {
        public override void Configure(EntityTypeBuilder<FoodItem> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.Name)
                .IsRequired();

            builder.Property(p => p.Calories)
                .IsRequired();

            builder.Property(p => p.Carbohydrates)
                .IsRequired();

            builder.Property(p => p.Proteins)
                .IsRequired();

            builder.Property(p => p.Fats)
                .IsRequired();

            builder.Property(p => p.SaturatedFats)
                .IsRequired();

            builder.HasOne(fi => fi.Brand)
                .WithMany(b => b.FoodItems)
                .HasForeignKey(fi => fi.BrandId);

            builder.HasMany(fi => fi.DailyFoodItems)
                .WithOne(dfi => dfi.FoodItem)
                .HasForeignKey(dfi => dfi.FoodItemId)
                .IsRequired();
        }
    }
}
