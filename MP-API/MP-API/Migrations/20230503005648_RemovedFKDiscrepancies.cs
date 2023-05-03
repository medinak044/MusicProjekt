using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MP_API.Migrations
{
    /// <inheritdoc />
    public partial class RemovedFKDiscrepancies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_WorkspaceItems_WorkspaceItemId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId1",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceItems_AspNetUsers_OwnerId1",
                table: "WorkspaceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceItems_Projects_ProjectId",
                table: "WorkspaceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceItems_Workspaces_WorkspaceId",
                table: "WorkspaceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_AspNetUsers_OwnerId",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Workspaces_OwnerId",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceItems_OwnerId1",
                table: "WorkspaceItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceItems_ProjectId",
                table: "WorkspaceItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceItems_WorkspaceId",
                table: "WorkspaceItems");

            migrationBuilder.DropIndex(
                name: "IX_Projects_OwnerId1",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WorkspaceItemId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "WorkspaceItems");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "WorkspaceItemId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Workspaces",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Workspaces",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId1",
                table: "WorkspaceItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId1",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkspaceItemId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_OwnerId",
                table: "Workspaces",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceItems_OwnerId1",
                table: "WorkspaceItems",
                column: "OwnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceItems_ProjectId",
                table: "WorkspaceItems",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceItems_WorkspaceId",
                table: "WorkspaceItems",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId1",
                table: "Projects",
                column: "OwnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WorkspaceItemId",
                table: "AspNetUsers",
                column: "WorkspaceItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_WorkspaceItems_WorkspaceItemId",
                table: "AspNetUsers",
                column: "WorkspaceItemId",
                principalTable: "WorkspaceItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId1",
                table: "Projects",
                column: "OwnerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceItems_AspNetUsers_OwnerId1",
                table: "WorkspaceItems",
                column: "OwnerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceItems_Projects_ProjectId",
                table: "WorkspaceItems",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceItems_Workspaces_WorkspaceId",
                table: "WorkspaceItems",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_AspNetUsers_OwnerId",
                table: "Workspaces",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
