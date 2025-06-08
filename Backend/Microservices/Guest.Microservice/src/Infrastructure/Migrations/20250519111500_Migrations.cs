using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "guest",
                columns: table => new
                {
                    guestid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fullname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    phonenumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("guest_pkey", x => x.guestid);
                });

            migrationBuilder.CreateTable(
                name: "guestrole",
                columns: table => new
                {
                    roleid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rolename = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("guestrole_pkey", x => x.roleid);
                });

            migrationBuilder.CreateTable(
                name: "guestrolemapping",
                columns: table => new
                {
                    guestid = table.Column<int>(type: "integer", nullable: false),
                    roleid = table.Column<int>(type: "integer", nullable: false),
                    assignedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("guestrolemapping_pkey", x => new { x.guestid, x.roleid });
                    table.ForeignKey(
                        name: "guestrolemapping_guestid_fkey",
                        column: x => x.guestid,
                        principalTable: "guest",
                        principalColumn: "guestid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "guestrolemapping_roleid_fkey",
                        column: x => x.roleid,
                        principalTable: "guestrole",
                        principalColumn: "roleid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "guest_email_key",
                table: "guest",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "guestrole_rolename_key",
                table: "guestrole",
                column: "rolename",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_guestrolemapping_roleid",
                table: "guestrolemapping",
                column: "roleid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guestrolemapping");

            migrationBuilder.DropTable(
                name: "guest");

            migrationBuilder.DropTable(
                name: "guestrole");
        }
    }
}
