using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CesiZen_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthentification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                schema: "app",
                table: "SavedActivities");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "app",
                table: "Users",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                schema: "app",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                schema: "app",
                table: "Users",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                schema: "app",
                table: "Users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                schema: "app",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                schema: "app",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                schema: "app",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                schema: "app",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                schema: "app",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "app",
                table: "Users",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "app",
                table: "SavedActivities",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
