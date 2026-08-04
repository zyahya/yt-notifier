using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YTNotifier.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnToNextDigestAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastDigestSendAt",
                table: "AspNetUsers",
                newName: "NextDigestAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NextDigestAt",
                table: "AspNetUsers",
                newName: "LastDigestSendAt");
        }
    }
}
