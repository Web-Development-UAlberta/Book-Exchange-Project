using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class Carrier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "carriers",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carriers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "carrier_rates",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    carrier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    base_cost = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    cost_per_kg = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    cost_per_km = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carrier_rates", x => x.id);
                    table.ForeignKey(
                        name: "FK_carrier_rates_carriers_carrier_id",
                        column: x => x.carrier_id,
                        principalSchema: "public",
                        principalTable: "carriers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_carrier_rates_carrier_id",
                schema: "public",
                table: "carrier_rates",
                column: "carrier_id");

            migrationBuilder.CreateIndex(
                name: "IX_carriers_name",
                schema: "public",
                table: "carriers",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "carrier_rates",
                schema: "public");

            migrationBuilder.DropTable(
                name: "carriers",
                schema: "public");
        }
    }
}
