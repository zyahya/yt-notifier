using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YTNotifier.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoIdColumnInVideosTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoId",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Videos");
        }
    }
}
