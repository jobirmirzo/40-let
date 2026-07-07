using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace _40Let.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bot_users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bot_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "foods",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    image = table.Column<string>(type: "text", nullable: true),
                    discount = table.Column<int>(type: "integer", nullable: false),
                    has_discount = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    discount = table.Column<int>(type: "integer", nullable: false),
                    has_promo_code = table.Column<bool>(type: "boolean", nullable: false),
                    total_price = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bot_user_orders",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bot_user_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_bot_user_orders_bot_users_user_id",
                        column: x => x.user_id,
                        principalTable: "bot_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bot_user_orders_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "checks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    withdrawal = table.Column<double>(type: "double precision", nullable: false),
                    ordered_count = table.Column<long>(type: "bigint", nullable: false),
                    with_promo_code = table.Column<bool>(type: "boolean", nullable: false),
                    discounted_amount = table.Column<double>(type: "double precision", nullable: false),
                    discount = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checks", x => x.id);
                    table.ForeignKey(
                        name: "FK_checks_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    food_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<double>(type: "double precision", nullable: false),
                    discount = table.Column<int>(type: "integer", nullable: false),
                    total_price = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_items_foods_food_id",
                        column: x => x.food_id,
                        principalTable: "foods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bot_user_histories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    history_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bot_user_histories", x => x.id);
                    table.ForeignKey(
                        name: "FK_bot_user_histories_bot_users_user_id",
                        column: x => x.user_id,
                        principalTable: "bot_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bot_user_histories_checks_history_id",
                        column: x => x.history_id,
                        principalTable: "checks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bot_user_histories_history_id",
                table: "bot_user_histories",
                column: "history_id");

            migrationBuilder.CreateIndex(
                name: "IX_bot_user_histories_user_id_history_id",
                table: "bot_user_histories",
                columns: new[] { "user_id", "history_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bot_user_orders_order_id",
                table: "bot_user_orders",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_bot_user_orders_user_id_order_id",
                table: "bot_user_orders",
                columns: new[] { "user_id", "order_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_chat_id",
                table: "bot_users",
                column: "chat_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_phone_number",
                table: "bot_users",
                column: "phone_number");

            migrationBuilder.CreateIndex(
                name: "IX_checks_order_id",
                table: "checks",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_foods_category",
                table: "foods",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_food_id",
                table: "order_items",
                column: "food_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_created_at",
                table: "orders",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_orders_status",
                table: "orders",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bot_user_histories");

            migrationBuilder.DropTable(
                name: "bot_user_orders");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "checks");

            migrationBuilder.DropTable(
                name: "bot_users");

            migrationBuilder.DropTable(
                name: "foods");

            migrationBuilder.DropTable(
                name: "orders");
        }
    }
}
