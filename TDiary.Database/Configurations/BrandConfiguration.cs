using Microsoft.EntityFrameworkCore;
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
    public class BrandConfiguration : BaseConfiguration<Brand>
    {
        public override void Configure(EntityTypeBuilder<Brand> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.HasMany(p => p.FoodItems)
                .WithOne(fi => fi.Brand)
                .HasForeignKey(fi => fi.BrandId)
                .IsRequired();
        }
    }
}
