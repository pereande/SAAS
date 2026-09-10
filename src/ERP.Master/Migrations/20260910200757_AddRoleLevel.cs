using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Master.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "level",
                schema: "public",
                table: "roles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "level",
                schema: "public",
                table: "roles");
        }
    }
}
