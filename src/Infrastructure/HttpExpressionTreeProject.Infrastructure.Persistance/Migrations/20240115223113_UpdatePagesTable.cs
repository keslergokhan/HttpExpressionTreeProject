using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HttpExpressionTreeProject.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Pages",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Pages");
        }
    }
}
