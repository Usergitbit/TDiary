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
    public class DietProfileConfiguration : BaseConfiguration<DietProfile>
    {
        public override void Configure(EntityTypeBuilder<DietProfile> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.BirthDate)
                .IsRequired();

            builder.Property(p => p.DietFormula)
                .IsRequired();

            builder.Property(p => p.EndDateUtc)
                .IsRequired();

            builder.Property(p => p.Height)
                .IsRequired();

            builder.Property(p => p.Name)
                .IsRequired();

            builder.Property(p => p.ProfileType)
                .IsRequired();

            builder.Property(p => p.Sex)
                .IsRequired();

            builder.Property(p => p.TargetCalories)
                .IsRequired();

            builder.Property(p => p.NeckCircumference)
                .IsRequired();

            builder.Property(p => p.WaistCircumference)
                .IsRequired();

            builder.Property(p => p.TargetDefinedByUser)
                .IsRequired();

            builder.Property(p => p.StartDateUtc)
                .IsRequired();

            builder.Property(p => p.BodyFatPercentage)
                .IsRequired();

            builder.Property(p => p.BodyFatPercentageDefinedByUser)
                .IsRequired();

            builder.Property(p => p.ActivityLevel)
                .IsRequired();
        }

    }
}
