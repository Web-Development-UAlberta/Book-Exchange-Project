using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAddressToAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "public",
                table: "addresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_addresses_user_id_IsDefault",
                schema: "public",
                table: "addresses",
                columns: new[] { "user_id", "IsDefault" },
                unique: true,
                filter: "\"IsDefault\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_addresses_user_id_IsDefault",
                schema: "public",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "public",
                table: "addresses");
        }
    }
}
