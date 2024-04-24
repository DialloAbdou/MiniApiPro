using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspWebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDateNaissance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeNaissance",
                table: "Personnes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateDeNaissance",
                table: "Personnes");
        }
    }
}
