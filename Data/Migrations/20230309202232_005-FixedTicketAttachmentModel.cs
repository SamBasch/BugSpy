using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugSpy.Data.Migrations
{
    /// <inheritdoc />
    public partial class _005FixedTicketAttachmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "TicketAttachments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TicketAttachments",
                type: "character varying(600)",
                maxLength: 600,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(600)",
                oldMaxLength: 600);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "TicketAttachments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TicketAttachments",
                type: "character varying(600)",
                maxLength: 600,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(600)",
                oldMaxLength: 600,
                oldNullable: true);
        }
    }
}
