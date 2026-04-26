using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class Review : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reviews",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id);
                    table.CheckConstraint("CK_reviews_rating_range", "\"rating\" BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_reviews_reviewer_not_reviewee", "\"reviewer_id\" <> \"reviewee_id\"");
                    table.ForeignKey(
                        name: "FK_reviews_AspNetUsers_reviewee_id",
                        column: x => x.reviewee_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviews_AspNetUsers_reviewer_id",
                        column: x => x.reviewer_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviews_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalSchema: "public",
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reviews_reviewee_id",
                schema: "public",
                table: "reviews",
                column: "reviewee_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_reviewer_id",
                schema: "public",
                table: "reviews",
                column: "reviewer_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_transaction_id_reviewer_id_reviewee_id",
                schema: "public",
                table: "reviews",
                columns: new[] { "transaction_id", "reviewer_id", "reviewee_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reviews",
                schema: "public");
        }
    }
}
