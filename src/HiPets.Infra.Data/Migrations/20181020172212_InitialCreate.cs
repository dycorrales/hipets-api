using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HiPets.Infra.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adopter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2018, 10, 20, 14, 22, 12, 400, DateTimeKind.Local)),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(250)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(11)", nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adopter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Animal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2018, 10, 20, 14, 22, 12, 366, DateTimeKind.Local)),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(250)", nullable: false),
                    Breed = table.Column<string>(type: "varchar(50)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    PrevalentColor = table.Column<int>(type: "int", nullable: false),
                    Behavior = table.Column<string>(type: "varchar(250)", nullable: false),
                    AnimalStatus = table.Column<int>(type: "int", nullable: false),
                    AnimalType = table.Column<int>(type: "int", nullable: false),
                    AdoptionDate = table.Column<DateTime>(nullable: true),
                    AdopterId = table.Column<Guid>(nullable: true),
                    PictureUrl = table.Column<string>(type: "varchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animal_Adopter_AdopterId",
                        column: x => x.AdopterId,
                        principalTable: "Adopter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Adoption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2018, 10, 20, 14, 22, 12, 411, DateTimeKind.Local)),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AnimalId = table.Column<Guid>(nullable: false),
                    AdopterId = table.Column<Guid>(nullable: false),
                    AdoptionStatus = table.Column<int>(type: "int", nullable: false),
                    Observation = table.Column<string>(type: "varchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adoption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adoption_Adopter_AdopterId",
                        column: x => x.AdopterId,
                        principalTable: "Adopter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Adoption_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adoption_AdopterId",
                table: "Adoption",
                column: "AdopterId");

            migrationBuilder.CreateIndex(
                name: "IX_Adoption_AnimalId",
                table: "Adoption",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Animal_AdopterId",
                table: "Animal",
                column: "AdopterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adoption");

            migrationBuilder.DropTable(
                name: "Animal");

            migrationBuilder.DropTable(
                name: "Adopter");
        }
    }
}
