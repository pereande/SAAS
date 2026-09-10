using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Master.Migrations
{
    /// <inheritdoc />
    public partial class FixUserRoleRoleFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_roles_role_id1",
                schema: "public",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "ix_user_roles_role_id1",
                schema: "public",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "role_id1",
                schema: "public",
                table: "user_roles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "role_id1",
                schema: "public",
                table: "user_roles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id1",
                schema: "public",
                table: "user_roles",
                column: "role_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_roles_role_id1",
                schema: "public",
                table: "user_roles",
                column: "role_id1",
                principalSchema: "public",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
