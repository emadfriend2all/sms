using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Tenant
{
    /// <inheritdoc />
    public partial class AddedAddressBankPhoneNumberInTenantInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "MultiTenancy",
                table: "Tenants");
        }
    }
}
