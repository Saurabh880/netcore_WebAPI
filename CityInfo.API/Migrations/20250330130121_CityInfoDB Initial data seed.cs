﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CityInfo.API.Migrations
{
    /// <inheritdoc />
    public partial class CityInfoDBInitialdataseed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CityInfos",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "The one with that big park.", "New York City" },
                    { 2, "The one with the cathedral that was never really finished.", "Antwerp" },
                    { 3, "The one with that big tower.", "Paris" }
                });

            migrationBuilder.InsertData(
                table: "PointsOfInterest",
                columns: new[] { "Id", "CityId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 1, "The most visited urban park in the United States.", "Central Park" },
                    { 2, 1, "A 102-story skyscraper located in Midtown Manhattan.", "Empire State Building" },
                    { 3, 2, "A Gothic style cathedral, conceived by architects Jan and Pieter Appelmans.", "Cathedral of Our Lady" },
                    { 4, 2, "The the finest example of railway architecture in Belgium.", "Antwerp Central Station" },
                    { 5, 3, "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel.", "Eiffel Tower" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PointsOfInterest",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PointsOfInterest",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PointsOfInterest",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PointsOfInterest",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PointsOfInterest",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CityInfos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CityInfos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CityInfos",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
