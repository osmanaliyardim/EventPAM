using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventPAM.Identity.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "operation_claims",
                columns: table => new
                {
                    ıd = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_operation_claims", x => x.ıd);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    ıd = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    password_hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    authenticator_type = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    ıs_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.ıd);
                });

            migrationBuilder.CreateTable(
                name: "email_authenticators",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_ıd = table.Column<Guid>(type: "uuid", nullable: false),
                    activation_key = table.Column<string>(type: "text", nullable: true),
                    ıs_verified = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_authenticators", x => x.id);
                    table.ForeignKey(
                        name: "fk_email_authenticators_users_user_id",
                        column: x => x.user_ıd,
                        principalTable: "users",
                        principalColumn: "ıd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "otp_authenticators",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_ıd = table.Column<Guid>(type: "uuid", nullable: false),
                    secret_key = table.Column<byte[]>(type: "bytea", nullable: false),
                    ıs_verified = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_otp_authenticators", x => x.id);
                    table.ForeignKey(
                        name: "fk_otp_authenticators_users_user_id",
                        column: x => x.user_ıd,
                        principalTable: "users",
                        principalColumn: "ıd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    ıd = table.Column<Guid>(type: "uuid", nullable: false),
                    user_ıd = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by_ıp = table.Column<string>(type: "text", nullable: true),
                    revoked = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    revoked_by_ıp = table.Column<string>(type: "text", nullable: true),
                    replaced_by_token = table.Column<string>(type: "text", nullable: true),
                    reason_revoked = table.Column<string>(type: "text", nullable: true),
                    user_id1 = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.ıd);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_ıd,
                        principalTable: "users",
                        principalColumn: "ıd");
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id1",
                        column: x => x.user_id1,
                        principalTable: "users",
                        principalColumn: "ıd");
                });

            migrationBuilder.CreateTable(
                name: "user_operation_claims",
                columns: table => new
                {
                    ıd = table.Column<Guid>(type: "uuid", nullable: false),
                    user_ıd = table.Column<Guid>(type: "uuid", nullable: false),
                    operation_claim_ıd = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_operation_claims", x => x.ıd);
                    table.ForeignKey(
                        name: "fk_user_operation_claims_operation_claims_operation_claim_id",
                        column: x => x.operation_claim_ıd,
                        principalTable: "operation_claims",
                        principalColumn: "ıd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_operation_claims_users_user_id",
                        column: x => x.user_ıd,
                        principalTable: "users",
                        principalColumn: "ıd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "operation_claims",
                columns: new[] { "ıd", "created_at", "created_by", "is_deleted", "last_modified", "last_modified_by", "name", "version" },
                values: new object[,]
                {
                    { new Guid("4c16d9d6-b050-4068-8422-a1828c405dda"), null, null, false, null, null, "Admin", 0L },
                    { new Guid("afa4fd58-f96f-4dac-92c2-b3357ab497d3"), null, null, false, null, null, "EventManager", 0L },
                    { new Guid("cc404012-70fa-4374-a317-157ea0784d56"), null, null, false, null, null, "Customer", 0L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_email_authenticators_user_id",
                table: "email_authenticators",
                column: "user_ıd");

            migrationBuilder.CreateIndex(
                name: "ix_operation_claims_name",
                table: "operation_claims",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_otp_authenticators_user_id",
                table: "otp_authenticators",
                column: "user_ıd");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_ıd",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id1",
                table: "refresh_tokens",
                column: "user_id1");

            migrationBuilder.CreateIndex(
                name: "ix_user_operation_claims_operation_claim_id",
                table: "user_operation_claims",
                column: "operation_claim_ıd");

            migrationBuilder.CreateIndex(
                name: "ix_user_operation_claims_user_id_operation_claim_id",
                table: "user_operation_claims",
                columns: new[] { "user_ıd", "operation_claim_ıd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_authenticators");

            migrationBuilder.DropTable(
                name: "otp_authenticators");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "user_operation_claims");

            migrationBuilder.DropTable(
                name: "operation_claims");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
