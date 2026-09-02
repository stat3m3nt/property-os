using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyOS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRowLevelSecurityScaffolding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE EXTENSION IF NOT EXISTS "pgcrypto";
                """);

            migrationBuilder.Sql("""
                CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP EXTENSION IF EXISTS "uuid-ossp";
                """);

            migrationBuilder.Sql("""
                DROP EXTENSION IF EXISTS "pgcrypto";
                """);
        }
    }
}
