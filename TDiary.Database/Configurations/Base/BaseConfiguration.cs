﻿using Microsoft.EntityFrameworkCore;
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

            builder.Property(p => p.CreatedAt)
                 .IsRequired();

            builder.Property(p => p.CreatedAtUtc)
                 .IsRequired();

            builder.Property(p => p.TimeZone)
                 .IsRequired();

            builder.Property(p => p.UserId)
                .IsRequired();
        }
    }
}
