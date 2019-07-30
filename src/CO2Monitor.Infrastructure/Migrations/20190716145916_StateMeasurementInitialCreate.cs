using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CO2Monitor.Infrastructure.Migrations {
	public partial class StateMeasurementInitialCreate : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "Measurements",
				columns: table => new {
					Id = table.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
					DeviceId = table.Column<int>(nullable: false),
					Time = table.Column<DateTime>(nullable: false),
					State = table.Column<string>(nullable: true)
				},
				constraints: table => table.PrimaryKey("PK_Measurements", x => x.Id));

			migrationBuilder.CreateIndex(
				name: "IX_Measurements_DeviceId",
				table: "Measurements",
				column: "DeviceId");

			migrationBuilder.CreateIndex(
				name: "IX_Measurements_Time",
				table: "Measurements",
				column: "Time");

			migrationBuilder.CreateIndex(
				name: "IX_Measurements_DeviceId_Time",
				table: "Measurements",
				columns: new[] { "DeviceId", "Time" });
		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "Measurements");
		}
	}
}
