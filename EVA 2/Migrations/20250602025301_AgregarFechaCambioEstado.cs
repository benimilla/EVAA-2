using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVA_2.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFechaCambioEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCambioEstado",
                table: "Citas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCambioEstado",
                table: "Citas");
        }
    }
}
