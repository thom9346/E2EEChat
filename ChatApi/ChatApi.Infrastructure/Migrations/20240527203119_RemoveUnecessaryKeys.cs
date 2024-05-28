using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnecessaryKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SigningPublicKey",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RequesteeSigningPublicKey",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "RequesterSigningPublicKey",
                table: "Friendships");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SigningPublicKey",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequesteeSigningPublicKey",
                table: "Friendships",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequesterSigningPublicKey",
                table: "Friendships",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
