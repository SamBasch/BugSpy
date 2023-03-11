using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugSpy.Data.Migrations
{
    /// <inheritdoc />
    public partial class _003DeletedFileNameFromTicketAttachmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "TicketAttachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "TicketAttachments",
                type: "text",
                nullable: true);
        }
    }
}
