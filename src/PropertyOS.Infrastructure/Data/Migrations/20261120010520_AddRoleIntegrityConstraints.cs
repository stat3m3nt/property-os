using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyOS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleIntegrityConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_roles_Global_Name",
                table: "roles",
                column: "Name",
                unique: true,
                filter: "\"OrganizationId\" IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_roles_SystemRoleOrganization",
                table: "roles",
                sql: "(\"OrganizationId\" IS NULL AND \"IsSystemRole\" = TRUE)\r\nOR\r\n(\"OrganizationId\" IS NOT NULL AND \"IsSystemRole\" = FALSE)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roles_Global_Name",
                table: "roles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_roles_SystemRoleOrganization",
                table: "roles");
        }
    }
}
