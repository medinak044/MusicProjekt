using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MP_API.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedWorkspaceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "WorkspaceItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "WorkspaceItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
