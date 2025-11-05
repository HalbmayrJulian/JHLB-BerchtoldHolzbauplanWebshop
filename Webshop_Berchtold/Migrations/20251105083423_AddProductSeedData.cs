using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Webshop_Berchtold.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Anzahl", "Beschreibung", "BildUrl", "ErstellungsDatum", "IstVerfuegbar", "KategorieId", "Name", "Preis" },
                values: new object[,]
                {
                    { 1, 100, "Edelstahl A2, rostfrei, Teilgewinde für optimalen Halt", "/images/products/screws1.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 577, DateTimeKind.Local).AddTicks(387), true, 1, "Holzschrauben Premium 6x80mm", 24.99m },
                    { 2, 50, "Verzinkte Balkenschuhe für sichere Holzverbindungen", "/images/products/beam-shoes.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(900), true, 1, "Balkenschuhe 80x120mm", 8.50m },
                    { 3, 30, "Verzinkte Gewindestangen verschiedene Längen für Holzkonstruktionen", "/images/products/threaded-rods.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(953), true, 1, "Gewindestangen M12", 15.60m },
                    { 4, 200, "Verschiedene Größen für optimale Kraftverteilung", "/images/products/washers.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(958), true, 1, "Unterlegscheiben Set", 5.90m },
                    { 5, 80, "Trockenes Fichtenholz, kammergetrocknet für tragende Zwecke", "/images/products/construction-wood.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(962), true, 2, "Konstruktionsholz 60x120mm", 8.90m },
                    { 6, 120, "Gehobelte Fichtenbretter für Schalung und Verkleidung", "/images/products/boards.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(965), true, 2, "Bretter 24x200mm", 12.50m },
                    { 7, 40, "Verleimte Fichtenbinder für tragende Zwecke im Holzbau", "/images/products/glulam-beams.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(968), true, 2, "Leimholzbalken 100x200mm", 28.90m },
                    { 8, 60, "Orientierte Spanplatten 2500x1250mm für Dach und Wand", "/images/products/osb-plates.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(971), true, 2, "OSB-Platten 18mm", 34.90m },
                    { 9, 25, "Professioneller Hammer mit Eschenstiel und Nagelzieher", "/images/products/hammer.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(974), true, 3, "Zimmermannshammer 600g", 45.90m },
                    { 10, 15, "18V Li-Ion mit 2 Akkus und Ladegerät für professionelle Anwendungen", "/images/products/cordless-drill.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(977), true, 3, "Akkuschrauber Set", 189.90m },
                    { 11, 35, "Präzisionswinkel für exakte Messungen im Holzbau", "/images/products/square.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(980), true, 3, "Zimmermannswinkel 600mm", 29.90m },
                    { 12, 20, "Aluminium-Wasserwaage mit 3 Libellen für präzises Arbeiten", "/images/products/level.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(983), true, 3, "Wasserwaage 1200mm", 67.90m },
                    { 13, 45, "Wetterschutzlasur für Außenholz mit UV-Schutz", "/images/products/wood-stain.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(987), true, 5, "Holzlasur Nussbaum 5L", 49.90m },
                    { 14, 60, "Natürliches Holzöl für Terrassen und Gartenmöbel", "/images/products/wood-oil.jpg", new DateTime(2025, 11, 5, 9, 34, 22, 580, DateTimeKind.Local).AddTicks(990), true, 5, "Holzöl Teak 2,5L", 38.50m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);
        }
    }
}
