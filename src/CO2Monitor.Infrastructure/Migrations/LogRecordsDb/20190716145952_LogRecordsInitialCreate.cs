using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CO2Monitor.Infrastructure.Migrations.LogRecordsDb {
	public partial class LogRecordsInitialCreate : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "Records",
				columns: table => new {
					Id = table.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
					EventId = table.Column<int>(nullable: true),
					LogLevel = table.Column<string>(nullable: true),
					Message = table.Column<string>(nullable: true),
					Time = table.Column<DateTime>(nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_Records", x => x.Id);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Records_Time",
				table: "Records",
				column: "Time");
		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "Records");
		}
	}
}
