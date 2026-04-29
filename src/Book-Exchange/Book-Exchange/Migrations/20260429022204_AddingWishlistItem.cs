using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class AddingWishlistItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wishlist",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    isbn = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wishlist", x => x.id);
                    table.CheckConstraint("ck_wishlist_isbn", "isbn ~ '^[0-9]{13}$' OR isbn ~ '^[0-9X]{10}$'");
                    table.ForeignKey(
                        name: "FK_wishlist_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_wishlist_isbn",
                schema: "public",
                table: "wishlist",
                column: "isbn");

            migrationBuilder.CreateIndex(
                name: "ix_wishlist_user_id",
                schema: "public",
                table: "wishlist",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_wishlist_user_id_isbn",
                schema: "public",
                table: "wishlist",
                columns: new[] { "user_id", "isbn" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wishlist",
                schema: "public");
        }
    }
}
