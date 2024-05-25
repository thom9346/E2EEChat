using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSignPublicKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SigningPublicKey",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SigningPublicKey",
                table: "Messages");
        }
    }
}
