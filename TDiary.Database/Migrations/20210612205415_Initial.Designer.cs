﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TDiary.Database;

namespace TDiary.Database.Migrations
{
    [DbContext(typeof(TDiaryDatabaseContext))]
    [Migration("20210612205415_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("TDiary.Common.Models.Entities.Brand", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LocallyCreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LocallyUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("TDiary.Common.Models.Entities.DailyFoodItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Calories")
                        .HasColumnType("double precision");

                    b.Property<double>("Carbohydrates")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<double>("Fats")
                        .HasColumnType("double precision");

                    b.Property<Guid>("FoodItemId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LocallyCreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LocallyUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("PlateId")
                        .HasColumnType("uuid");

                    b.Property<double>("Proteins")
                        .HasColumnType("double precision");

                    b.Property<double>("Quantity")
                        .HasColumnType("double precision");

                    b.Property<double>("SaturatedFats")
                        .HasColumnType("double precision");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("FoodItemId");

                    b.HasIndex("PlateId");

                    b.ToTable("DailyFoodItems");
                });

            modelBuilder.Entity("TDiary.Common.Models.Entities.DietProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("ActivityLevel")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("BodyFatPercentage")
                        .HasColumnType("double precision");

                    b.Property<bool>("BodyFatPercentageDefinedByUser")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<int>("DietFormula")
                        .HasColumnType("integer");

                    b.Property<DateTime>("EndDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("Height")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("LocallyCreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LocallyUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("NeckCircumference")
                        .HasColumnType("double precision");

                    b.Property<int>("ProfileType")
                        .HasColumnType("integer");

                    b.Property<int>("Sex")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("TargetCalories")
                        .HasColumnType("integer");

                    b.Property<bool>("TargetDefinedByUser")
                        .HasColumnType("boolean");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<double>("WaistCircumference")
                        .HasColumnType("double precision");

                    b.Property<double>("Weight")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("DietProfiles");
                });

            modelBuilder.Entity("TDiary.Common.Models.Entities.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Entity")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("EventType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LocallyCreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LocallyUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("TDiary.Common.Models.Entities.FoodItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BrandId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<double>("Calories")
                        .HasColumnType("double precision");

                    b.Property<double>("Carbohydrates")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<double>("Fats")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("LocallyCreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LocallyUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Proteins")
                        .HasColumnType("double precision");

                    b.Property<double>("SaturatedFats")
                        .HasColumnType("double precision");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.ToTable("FoodItems");
                });

            modelBuilder.Entity("TDiary.Common.Models.Entities.Plate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LocallyCreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LocallyUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<double>("Weight")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Plates");
                });

            modelBuilder.Entity("TDiary.Common.Models.Entities.DailyFoodItem", b =>
                {
                    b.HasOne("TDiary.Common.Models.Entities.FoodItem", "FoodItem")
                        .WithMany("DailyFoodItems")
                        .HasForeignKey("FoodItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TDiary.Common.Models.Entities.Plate", "Plate")
                        .WithMany("DailyFoodItems")
                        .HasForeignKey("PlateId");

                    b.Navigation("FoodItem");

                    b.Navigation("Plate");
                });

            modelBuilder.Entity("TDiary.Common.Models.Entities.FoodItem", b =>
                {
                    b.HasOne("TDiary.Common.Models.Entities.Brand", "Brand")
                        .WithMany("FoodItems")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");
                });

            modelBuilder.Entity("TDiary.Common.Models.Entities.Brand", b =>
                {
                    b.Navigation("FoodItems");
                });

            modelBuilder.Entity("TDiary.Common.Models.Entities.FoodItem", b =>
                {
                    b.Navigation("DailyFoodItems");
                });

            modelBuilder.Entity("TDiary.Common.Models.Entities.Plate", b =>
                {
                    b.Navigation("DailyFoodItems");
                });
#pragma warning restore 612, 618
        }
    }
}
