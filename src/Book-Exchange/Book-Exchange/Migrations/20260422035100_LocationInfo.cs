using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class LocationInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "locations",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    province_state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "location_distances",
                schema: "public",
                columns: table => new
                {
                    from_location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    distance_km = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_distances", x => new { x.from_location_id, x.to_location_id });
                    table.CheckConstraint("CK_location_distances_distance_nonnegative", "\"distance_km\" >= 0");
                    table.CheckConstraint("CK_location_distances_not_same", "\"from_location_id\" <> \"to_location_id\"");
                    table.ForeignKey(
                        name: "FK_location_distances_locations_from_location_id",
                        column: x => x.from_location_id,
                        principalSchema: "public",
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_location_distances_locations_to_location_id",
                        column: x => x.to_location_id,
                        principalSchema: "public",
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_location_distances_to_location_id",
                schema: "public",
                table: "location_distances",
                column: "to_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_locations_city_province_state_country",
                schema: "public",
                table: "locations",
                columns: new[] { "city", "province_state", "country" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "location_distances",
                schema: "public");

            migrationBuilder.DropTable(
                name: "locations",
                schema: "public");
        }
    }
}
