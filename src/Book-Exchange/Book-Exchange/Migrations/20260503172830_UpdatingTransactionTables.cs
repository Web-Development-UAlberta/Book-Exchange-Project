using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingTransactionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                schema: "public",
                table: "transactions");

            migrationBuilder.CreateTable(
                name: "transaction_status_history",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "transaction_status", nullable: false, defaultValueSql: "'confirmed'::transaction_status"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaction_status_history_asp_net_users_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transaction_status_history_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalSchema: "public",
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transaction_status_history_transaction_id",
                schema: "public",
                table: "transaction_status_history",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_status_history_updated_by_user_id",
                schema: "public",
                table: "transaction_status_history",
                column: "updated_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transaction_status_history",
                schema: "public");

            migrationBuilder.AddColumn<int>(
                name: "status",
                schema: "public",
                table: "transactions",
                type: "transaction_status",
                nullable: false,
                defaultValueSql: "'confirmed'::transaction_status");
        }
    }
}
