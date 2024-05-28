using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSigningKeysToFriendship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequesteeSigningPublicKey",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "RequesterSigningPublicKey",
                table: "Friendships");
        }
    }
}
