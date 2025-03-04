﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeuroTrade.Migrations
{
    /// <inheritdoc />
    public partial class changedStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Stocks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
