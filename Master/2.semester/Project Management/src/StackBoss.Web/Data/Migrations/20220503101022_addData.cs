using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StackBoss.Web.Data.Migrations
{
    public partial class addData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Manager = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Staff = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiskTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Threat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Starters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reaction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Probability = table.Column<int>(type: "int", nullable: false),
                    Consequences = table.Column<int>(type: "int", nullable: false),
                    RiskEvaluation = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedStateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    CustomId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskTable_ProjectTable_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ProjectTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b6fb5986-c33a-438a-bb5d-caed50925624", "d65c6a25-b7d8-4c04-8133-4cfb222f5f12", "Admin", "ADMIN" },
                    { "e2ed6b05-6503-4f88-90d3-84e28c7b6a65", "993d4ebb-c483-4182-add6-04da1aaea5fb", "ProjectManager", "PROJECTMANAGER" },
                    { "9e4a5add-9f62-4512-9e1e-725a6e93523d", "37c9288c-f60e-4afa-aa21-bffd7319f8e0", "ProjectDirector", "PROJECTDIRECTOR" }
                });

            migrationBuilder.InsertData(
                table: "ProjectTable",
                columns: new[] { "Id", "CustomId", "Description", "Manager", "Name", "Staff" },
                values: new object[,]
                {
                    { 1, "P_001", "Information system for Hospital in Brno", "Ing. Jan Honza", "Medical IS", "Lukas Kudlicka, Michal Kovac" },
                    { 2, "P_002", "Information system for Andrews Constructions", "Ing. ´Michal Slivka", "Engineering IS", "Peter Janosik, Michal Kutil" },
                    { 3, "P_003", "Information system for Hotel Yasmin", "Ing. ´Michal Chrapko", "Hotel IS", "Jakub Varga, Jakub Kulan" }
                });

            migrationBuilder.InsertData(
                table: "RiskTable",
                columns: new[] { "Id", "Category", "Consequences", "CreatedDate", "CustomId", "Description", "ModifiedStateDate", "Name", "Owner", "Probability", "ProjectId", "Reaction", "ReactionDate", "RiskEvaluation", "Starters", "State", "Threat" },
                values: new object[,]
                {
                    { 1, 2, 8, new DateTime(2021, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "P001_R01", "Test bussiness risk", new DateTime(2021, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of Losing customers", "Ing. Jozko Mrkvicka", 3, 1, "Have 2 or more suppliers", new DateTime(2022, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 24, "Lack of medication", 1, "The supplier will not supply the medication" },
                    { 2, 3, 7, new DateTime(2022, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "P001_R02", "Test extern risk", new DateTime(2022, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of National Crisis", "Ing. Jozko Mrkvicka", 3, 1, "Create a reserve fund", new DateTime(2022, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 48, "People stopped buying medication enough", 1, "Loosing all of money" },
                    { 3, 0, 9, new DateTime(2021, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "P001_R03", "Test technical risk", new DateTime(2021, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of Lost Data", "Ing. Jozko Mrkvicka", 4, 1, "Buy and install antivirus", new DateTime(2021, 11, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 36, "The system was not checked against hacking", 1, "Loosing all of data" },
                    { 4, 1, 8, new DateTime(2021, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "P001_R04", "Test project risk", new DateTime(2021, 10, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of Planning", "Ing. Jozko Mrkvicka", 1, 1, "Increase employee control", new DateTime(2022, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, "Insufficient control of employees", 2, "Loss of control of the project" },
                    { 5, 1, 7, new DateTime(2021, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "P001_R06", "Test project risk", new DateTime(2021, 8, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of System Failure", "Ing. Jozko Mrkvicka", 4, 1, "Test the system as a whole but also in parts", new DateTime(2021, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 28, "Not every part of the system has been tested", 2, "The whole system will fall" },
                    { 6, 0, 8, new DateTime(2021, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "P002_R06", "Test technical risk", new DateTime(2021, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of Lost Data", "Ing. Jan Hrasko", 4, 2, "Buy and install antivirus", new DateTime(2021, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 32, "The system was not checked against hacking", 1, "Loosing all of data" },
                    { 7, 2, 6, new DateTime(2021, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "P002_R07", "Test business risk", new DateTime(2021, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of Losing market position", "Ing. Jan Hrasko", 4, 2, "translate the manual into several languages", new DateTime(2021, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 24, "Product manual only in national language", 1, "The foreign market will know nothing about us" },
                    { 8, 3, 9, new DateTime(2021, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "P002_R08", "Test extern risk", new DateTime(2021, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of National Crisis", "Ing. Jan Hrasko", 5, 2, "Create a reserve fund", new DateTime(2022, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 45, "People stopped buying medication enough", 1, "Loosing all of money" },
                    { 9, 0, 10, new DateTime(2021, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "P002_R09", "Test technical risk", new DateTime(2021, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of Fire", "Ing. Jan Hrasko", 6, 2, "Buy and inspect fire detectors regularly", new DateTime(2021, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 60, "The factory does not have fire detectors", 1, "loss of production site" },
                    { 10, 1, 6, new DateTime(2021, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "P002_R10", "Test project risk", new DateTime(2021, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of Planning", "Ing. Jan Hrasko", 2, 2, "Increase employee control", new DateTime(2022, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, "Insufficient control of employees", 4, "Loss of control of the project" },
                    { 11, 1, 7, new DateTime(2021, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "P002_R11", "Test project risk", new DateTime(2021, 8, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of System Failure", "Ing. Jan Hrasko", 4, 2, "Test the system as a whole but also in parts", new DateTime(2021, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 28, "Not every part of the system has been tested", 2, "The whole system will fall" },
                    { 12, 0, 5, new DateTime(2021, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "P003_R12", "Test technical risk", new DateTime(2021, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of losing information about the accommodated", "Ing. Jakub Malik", 5, 3, "Buy and install antivirus", new DateTime(2021, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 25, "The system was not checked against hacking", 1, "Loosing all of data" },
                    { 13, 2, 6, new DateTime(2021, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "P003_R13", "Test business risk", new DateTime(2021, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "risk of Losing the position on the international market", "Ing. Jakub Malik", 3, 3, "Create a page for online booking", new DateTime(2021, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 18, "The hotel was not sufficiently presented online", 1, "The foreign market will know nothing about us" },
                    { 14, 0, 10, new DateTime(2021, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "P003_R14", "Test technical risk", new DateTime(2021, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of Fire", "Ing. Jakub Malik", 6, 3, "Buy and inspect fire detectors regularly", new DateTime(2021, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 60, "The hotel does not have fire detectors", 1, "Loss of space for customer accommodation" },
                    { 15, 1, 7, new DateTime(2021, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "P003_R15", "Test project risk", new DateTime(2021, 8, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of System Failure", "Ing. Jakub Malik", 2, 3, "Test the system as a whole but also in parts", new DateTime(2021, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 28, "Not every part of the system has been tested", 3, "Impossibility to accommodate anyone" },
                    { 16, 3, 8, new DateTime(2021, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "P003_R16", "Test extern risk", new DateTime(2021, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of National Crisis", "Ing. Jakub Malik", 4, 3, "Create a reserve fund", new DateTime(2022, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 32, "People stopped going on trips with accommodation", 1, "Lost most of money" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RiskTable_ProjectId",
                table: "RiskTable",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RiskTable");

            migrationBuilder.DropTable(
                name: "ProjectTable");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9e4a5add-9f62-4512-9e1e-725a6e93523d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b6fb5986-c33a-438a-bb5d-caed50925624");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e2ed6b05-6503-4f88-90d3-84e28c7b6a65");
        }
    }
}
