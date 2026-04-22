using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class Book : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "books",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    isbn_13 = table.Column<string>(type: "char(13)", maxLength: 13, nullable: true),
                    isbn_10 = table.Column<string>(type: "char(10)", maxLength: 10, nullable: true),
                    published_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_books_isbn_10",
                schema: "public",
                table: "books",
                column: "isbn_10",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_books_isbn_13",
                schema: "public",
                table: "books",
                column: "isbn_13",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "books",
                schema: "public");
        }
    }
}
