using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webshop_Berchtold.Migrations
{
    /// <inheritdoc />
    public partial class AddEinheitAndIconToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Einheit",
                table: "Products",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IconClass",
                table: "Products",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Beschreibung", "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { "Edelstahl A2, rostfrei, Teilgewinde", null, "pro 100 Stk.", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-wrench" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { null, "pro Stück", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-nut" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Beschreibung", "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { "Verzinkte Gewindestangen verschiedene Längen", null, "pro Meter", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-paperclip" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { null, "pro Set", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-circle" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Beschreibung", "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { "Trockenes Fichtenholz, kammergetrocknet", null, "pro lfd. Meter", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-columns" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Beschreibung", "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { "Gehobelte Fichtenbretter für Schalung", null, "pro lfd. Meter", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-layout-three-columns" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Beschreibung", "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { "Verleimte Fichtenbinder für tragende Zwecke", null, "pro lfd. Meter", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-bricks" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Beschreibung", "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { "Orientierte Spanplatten 2500x1250mm", null, "pro Platte", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-square" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Beschreibung", "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { "Professioneller Hammer mit Eschenstiel", null, "pro Stück", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-hammer" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Beschreibung", "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { "18V Li-Ion mit 2 Akkus und Ladegerät", null, "pro Set", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-tools" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Beschreibung", "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { "Präzisionswinkel für exakte Messungen", null, "pro Stück", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-rulers" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Beschreibung", "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { "Aluminium-Wasserwaage mit 3 Libellen", null, "pro Stück", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-eyedropper" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { null, "pro 5 Liter", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-paint-bucket" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "BildUrl", "Einheit", "ErstellungsDatum", "IconClass" },
                values: new object[] { null, "pro 2,5 Liter", new DateTime(2025, 11, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "bi-droplet-fill" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Einheit",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IconClass",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Beschreibung", "BildUrl", "ErstellungsDatum" },
                values: new object[] { "Edelstahl A2, rostfrei, Teilgewinde für optimalen Halt", "/images/products/screws1.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 577, DateTimeKind.Local).AddTicks(387) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BildUrl", "ErstellungsDatum" },
                values: new object[] { "/images/products/beam-shoes.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(900) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Beschreibung", "BildUrl", "ErstellungsDatum" },
                values: new object[] { "Verzinkte Gewindestangen verschiedene Längen für Holzkonstruktionen", "/images/products/threaded-rods.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(953) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BildUrl", "ErstellungsDatum" },
                values: new object[] { "/images/products/washers.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(958) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Beschreibung", "BildUrl", "ErstellungsDatum" },
                values: new object[] { "Trockenes Fichtenholz, kammergetrocknet für tragende Zwecke", "/images/products/construction-wood.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(962) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Beschreibung", "BildUrl", "ErstellungsDatum" },
                values: new object[] { "Gehobelte Fichtenbretter für Schalung und Verkleidung", "/images/products/boards.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(965) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Beschreibung", "BildUrl", "ErstellungsDatum" },
                values: new object[] { "Verleimte Fichtenbinder für tragende Zwecke im Holzbau", "/images/products/glulam-beams.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(968) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Beschreibung", "BildUrl", "ErstellungsDatum" },
                values: new object[] { "Orientierte Spanplatten 2500x1250mm für Dach und Wand", "/images/products/osb-plates.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(971) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Beschreibung", "BildUrl", "ErstellungsDatum" },
                values: new object[] { "Professioneller Hammer mit Eschenstiel und Nagelzieher", "/images/products/hammer.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(974) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Beschreibung", "BildUrl", "ErstellungsDatum" },
                values: new object[] { "18V Li-Ion mit 2 Akkus und Ladegerät für professionelle Anwendungen", "/images/products/cordless-drill.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(977) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Beschreibung", "BildUrl", "ErstellungsDatum" },
                values: new object[] { "Präzisionswinkel für exakte Messungen im Holzbau", "/images/products/square.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(980) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Beschreibung", "BildUrl", "ErstellungsDatum" },
                values: new object[] { "Aluminium-Wasserwaage mit 3 Libellen für präzises Arbeiten", "/images/products/level.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(983) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "BildUrl", "ErstellungsDatum" },
                values: new object[] { "/images/products/wood-stain.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(987) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "BildUrl", "ErstellungsDatum" },
                values: new object[] { "/images/products/wood-oil.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(990) });
        }
    }
}
