using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Database.Configurations;

namespace TDiary.Database
{
    public class TDiaryDatabaseContext : DbContext
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<DailyFoodItem> DailyFoodItems { get; set; }
        public DbSet<DietProfile> DietProfiles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Plate> Plates { get; set; }

        public TDiaryDatabaseContext(DbContextOptions<TDiaryDatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
