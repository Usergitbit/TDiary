using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TDiary.Database.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedtAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DietProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Height = table.Column<double>(type: "double precision", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Sex = table.Column<int>(type: "integer", nullable: false),
                    DietFormula = table.Column<int>(type: "integer", nullable: false),
                    ActivityLevel = table.Column<double>(type: "double precision", nullable: false),
                    WaistCircumference = table.Column<double>(type: "double precision", nullable: false),
                    NeckCircumference = table.Column<double>(type: "double precision", nullable: false),
                    BodyFatPercentage = table.Column<double>(type: "double precision", nullable: false),
                    BodyFatPercentageDefinedByUser = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProfileType = table.Column<int>(type: "integer", nullable: false),
                    TargetCalories = table.Column<int>(type: "integer", nullable: false),
                    TargetDefinedByUser = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedtAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Entity = table.Column<string>(type: "text", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Changes = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<string>(type: "text", nullable: false),
                    InitialData = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedtAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedtAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Calories = table.Column<double>(type: "double precision", nullable: false),
                    Carbohydrates = table.Column<double>(type: "double precision", nullable: false),
                    Proteins = table.Column<double>(type: "double precision", nullable: false),
                    Fats = table.Column<double>(type: "double precision", nullable: false),
                    SaturatedFats = table.Column<double>(type: "double precision", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedtAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodItems_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyFoodItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Calories = table.Column<double>(type: "double precision", nullable: false),
                    Carbohydrates = table.Column<double>(type: "double precision", nullable: false),
                    Proteins = table.Column<double>(type: "double precision", nullable: false),
                    Fats = table.Column<double>(type: "double precision", nullable: false),
                    SaturatedFats = table.Column<double>(type: "double precision", nullable: false),
                    PlateId = table.Column<Guid>(type: "uuid", nullable: true),
                    FoodItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedtAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyFoodItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyFoodItems_FoodItems_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "FoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DailyFoodItems_Plates_PlateId",
                        column: x => x.PlateId,
                        principalTable: "Plates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyFoodItems_FoodItemId",
                table: "DailyFoodItems",
                column: "FoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyFoodItems_PlateId",
                table: "DailyFoodItems",
                column: "PlateId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatedAtUtc",
                table: "Events",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_FoodItems_BrandId",
                table: "FoodItems",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyFoodItems");

            migrationBuilder.DropTable(
                name: "DietProfiles");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "FoodItems");

            migrationBuilder.DropTable(
                name: "Plates");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
