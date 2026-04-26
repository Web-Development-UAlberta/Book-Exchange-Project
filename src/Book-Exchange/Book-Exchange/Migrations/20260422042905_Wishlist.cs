using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class Wishlist : Migration
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
                    book_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wishlist", x => x.id);
                    table.ForeignKey(
                        name: "FK_wishlist_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_wishlist_books_book_id",
                        column: x => x.book_id,
                        principalSchema: "public",
                        principalTable: "books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wishlist_book_id",
                schema: "public",
                table: "wishlist",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "IX_wishlist_user_id_book_id",
                schema: "public",
                table: "wishlist",
                columns: new[] { "user_id", "book_id" },
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
