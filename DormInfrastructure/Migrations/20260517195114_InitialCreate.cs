using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DormInfrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dorm",
                columns: table => new
                {
                    gu_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gu_nomer = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    gu_adresa = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    gu_kilkist_poverh = table.Column<int>(type: "integer", nullable: true),
                    gu_komendant = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gu_information = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("dorm_pkey", x => x.gu_id);
                });

            migrationBuilder.CreateTable(
                name: "faculty",
                columns: table => new
                {
                    fa_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fa_information = table.Column<string>(type: "text", nullable: true),
                    fa_telefon = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    fa_korpus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fa_dekan = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fa_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("faculty_pkey", x => x.fa_id);
                });

            migrationBuilder.CreateTable(
                name: "room",
                columns: table => new
                {
                    ki_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gu_id = table.Column<int>(type: "integer", nullable: false),
                    ki_information = table.Column<string>(type: "text", nullable: true),
                    ki_poverh = table.Column<int>(type: "integer", nullable: true),
                    ki_mistkist = table.Column<int>(type: "integer", nullable: true),
                    ki_nomer = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("room_pkey", x => x.ki_id);
                    table.ForeignKey(
                        name: "room_gu_id_fkey",
                        column: x => x.gu_id,
                        principalTable: "dorm",
                        principalColumn: "gu_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    ka_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fa_id = table.Column<int>(type: "integer", nullable: false),
                    ka_information = table.Column<string>(type: "text", nullable: true),
                    ka_telefon = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    ka_zaviduvach = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ka_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("department_pkey", x => x.ka_id);
                    table.ForeignKey(
                        name: "department_fa_id_fkey",
                        column: x => x.fa_id,
                        principalTable: "faculty",
                        principalColumn: "fa_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "student",
                columns: table => new
                {
                    st_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fa_id = table.Column<int>(type: "integer", nullable: false),
                    ka_id = table.Column<int>(type: "integer", nullable: false),
                    st_pib = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    st_kurs = table.Column<int>(type: "integer", nullable: true),
                    st_telefon = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    st_data_narodz = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("student_pkey", x => x.st_id);
                    table.ForeignKey(
                        name: "student_fa_id_fkey",
                        column: x => x.fa_id,
                        principalTable: "faculty",
                        principalColumn: "fa_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "student_ka_id_fkey",
                        column: x => x.ka_id,
                        principalTable: "department",
                        principalColumn: "ka_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accommodation",
                columns: table => new
                {
                    pr_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ki_id = table.Column<int>(type: "integer", nullable: false),
                    st_id = table.Column<int>(type: "integer", nullable: false),
                    pr_data_vysel = table.Column<DateOnly>(type: "date", nullable: true),
                    pr_data_zasel = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("accommodation_pkey", x => x.pr_id);
                    table.ForeignKey(
                        name: "accommodation_ki_id_fkey",
                        column: x => x.ki_id,
                        principalTable: "room",
                        principalColumn: "ki_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "accommodation_st_id_fkey",
                        column: x => x.st_id,
                        principalTable: "student",
                        principalColumn: "st_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_accommodation_ki_id",
                table: "accommodation",
                column: "ki_id");

            migrationBuilder.CreateIndex(
                name: "idx_accommodation_st_id",
                table: "accommodation",
                column: "st_id");

            migrationBuilder.CreateIndex(
                name: "idx_department_fa_id",
                table: "department",
                column: "fa_id");

            migrationBuilder.CreateIndex(
                name: "idx_room_gu_id",
                table: "room",
                column: "gu_id");

            migrationBuilder.CreateIndex(
                name: "idx_student_fa_id",
                table: "student",
                column: "fa_id");

            migrationBuilder.CreateIndex(
                name: "idx_student_ka_id",
                table: "student",
                column: "ka_id");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accommodation");

            migrationBuilder.DropTable(
                name: "room");

            migrationBuilder.DropTable(
                name: "student");

            migrationBuilder.DropTable(
                name: "dorm");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "faculty");
        }
    }
}
