using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectD.Migrations
{
    /// <inheritdoc />
    public partial class InitialFlightSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    TimetableId = table.Column<int>(type: "integer", nullable: false),
                    TrafficType = table.Column<string>(type: "text", nullable: false),
                    FlightNumber = table.Column<string>(type: "text", nullable: false),
                    Diverted = table.Column<bool>(type: "boolean", nullable: false),
                    Nachtvlucht = table.Column<bool>(type: "boolean", nullable: false),
                    FlightCode = table.Column<string>(type: "text", nullable: false),
                    FlightCodeDescription = table.Column<string>(type: "text", nullable: false),
                    FlightCodeIATA = table.Column<string>(type: "text", nullable: false),
                    PublicAnnouncement = table.Column<bool>(type: "boolean", nullable: false),
                    ScheduledUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Bewegingen = table.Column<string>(type: "text", nullable: false),
                    Parkeerpositie = table.Column<string>(type: "text", nullable: false),
                    Bus = table.Column<bool>(type: "boolean", nullable: false),
                    Gate = table.Column<string>(type: "text", nullable: false),
                    Bagageband = table.Column<int>(type: "integer", nullable: false),
                    AirportICAO = table.Column<string>(type: "text", nullable: false),
                    Airport = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    ViaAirportICAO = table.Column<string>(type: "text", nullable: true),
                    ViaAirport = table.Column<string>(type: "text", nullable: true),
                    AircraftRegistration = table.Column<string>(type: "text", nullable: false),
                    Seats = table.Column<int>(type: "integer", nullable: false),
                    MTOW = table.Column<int>(type: "integer", nullable: false),
                    AircraftType = table.Column<string>(type: "text", nullable: false),
                    AircraftDescription = table.Column<string>(type: "text", nullable: false),
                    EU = table.Column<bool>(type: "boolean", nullable: false),
                    Schengen = table.Column<bool>(type: "boolean", nullable: false),
                    AirlineFullname = table.Column<string>(type: "text", nullable: false),
                    AirlineShortname = table.Column<string>(type: "text", nullable: false),
                    AirlineICAO = table.Column<string>(type: "text", nullable: false),
                    AirlineIATA = table.Column<string>(type: "text", nullable: false),
                    Debiteur = table.Column<string>(type: "text", nullable: false),
                    DebiteurNr = table.Column<int>(type: "integer", nullable: false),
                    PaxMale = table.Column<int>(type: "integer", nullable: false),
                    PaxFemale = table.Column<int>(type: "integer", nullable: false),
                    PaxChild = table.Column<int>(type: "integer", nullable: false),
                    PaxInfant = table.Column<int>(type: "integer", nullable: false),
                    PaxTransitMale = table.Column<int>(type: "integer", nullable: false),
                    PaxTransitFemale = table.Column<int>(type: "integer", nullable: false),
                    PaxTransitChild = table.Column<int>(type: "integer", nullable: false),
                    PaxTransitInfant = table.Column<int>(type: "integer", nullable: false),
                    CrewCabin = table.Column<int>(type: "integer", nullable: false),
                    CrewCockpit = table.Column<int>(type: "integer", nullable: false),
                    BagsWeight = table.Column<double>(type: "double precision", nullable: false),
                    BagsTransitWeight = table.Column<double>(type: "double precision", nullable: false),
                    Bags = table.Column<int>(type: "integer", nullable: false),
                    BagsTransit = table.Column<int>(type: "integer", nullable: false),
                    Afhandelaar = table.Column<string>(type: "text", nullable: false),
                    ForecastPercentage = table.Column<double>(type: "double precision", nullable: false),
                    ForecastPax = table.Column<int>(type: "integer", nullable: false),
                    ForecastBabys = table.Column<int>(type: "integer", nullable: false),
                    FlightClass = table.Column<string>(type: "text", nullable: false),
                    Datasource = table.Column<string>(type: "text", nullable: false),
                    TotalPax = table.Column<int>(type: "integer", nullable: false),
                    TerminalPax = table.Column<int>(type: "integer", nullable: false),
                    TotalPaxBetalend = table.Column<int>(type: "integer", nullable: false),
                    TerminalPaxBetalend = table.Column<int>(type: "integer", nullable: false),
                    TransitPax = table.Column<int>(type: "integer", nullable: false),
                    TransitPaxBetalend = table.Column<int>(type: "integer", nullable: false),
                    TotalCrew = table.Column<int>(type: "integer", nullable: false),
                    TerminalCrew = table.Column<int>(type: "integer", nullable: false),
                    TotalSeats = table.Column<int>(type: "integer", nullable: false),
                    TerminalSeats = table.Column<int>(type: "integer", nullable: false),
                    TotalBags = table.Column<int>(type: "integer", nullable: false),
                    TerminalBags = table.Column<int>(type: "integer", nullable: false),
                    TransitBags = table.Column<int>(type: "integer", nullable: false),
                    TotalBagsWeight = table.Column<double>(type: "double precision", nullable: false),
                    TerminalBagsWeight = table.Column<double>(type: "double precision", nullable: false),
                    TransitBagsWeight = table.Column<double>(type: "double precision", nullable: false),
                    Runway = table.Column<string>(type: "text", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Elevation = table.Column<double>(type: "double precision", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    DistanceKilometers = table.Column<double>(type: "double precision", nullable: false),
                    Direction = table.Column<string>(type: "text", nullable: false),
                    AirportIATA = table.Column<string>(type: "text", nullable: false),
                    Parked = table.Column<bool>(type: "boolean", nullable: false),
                    Seizoen = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Touchpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FlightId = table.Column<int>(type: "integer", nullable: false),
                    TouchpointType = table.Column<string>(type: "text", nullable: false),
                    TouchpointTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TouchpointPax = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Touchpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Touchpoints_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Touchpoints_FlightId",
                table: "Touchpoints",
                column: "FlightId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Touchpoints");

            migrationBuilder.DropTable(
                name: "Flights");
        }
    }
}
