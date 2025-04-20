using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Migrations
{
    public partial class AddTaskStartEndTimeFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDateTime",
                table: "Todos",
                newName: "TaskStartTime");

            migrationBuilder.RenameColumn(
                name: "EndDateTime",
                table: "Todos",
                newName: "TaskEndTime");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 4, 13, 19, 37, 30, 702, DateTimeKind.Local).AddTicks(6256));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaskStartTime",
                table: "Todos",
                newName: "StartDateTime");

            migrationBuilder.RenameColumn(
                name: "TaskEndTime",
                table: "Todos",
                newName: "EndDateTime");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 4, 13, 19, 31, 52, 902, DateTimeKind.Local).AddTicks(8414));
        }
    }
}
