using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectD
{
    /// <summary>
    /// Vertegenwoordigt een vlucht met uitgebreide operationele, passagiers- en bagagegegevens.
    /// </summary>
    public class Flight
    {
        /// <summary>Type van het object (bijv. arrival, departure)</summary>
        public string Type { get; set; }

        /// <summary>Uniek identificatienummer van de vlucht</summary>
        public int Id { get; set; }

        /// <summary>Verwijzing naar het tijdschema</summary>
        public int TimetableId { get; set; }

        /// <summary>Soort verkeer (bijv. commercieel, cargo)</summary>
        public string TrafficType { get; set; }

        /// <summary>Vluchtnummer</summary>
        public string FlightNumber { get; set; }

        /// <summary>Geeft aan of de vlucht is omgeleid</summary>
        public bool Diverted { get; set; }

        /// <summary>Geeft aan of het een nachtvlucht betreft</summary>
        public bool Nachtvlucht { get; set; }

        /// <summary>Unieke code van de vlucht</summary>
        public string FlightCode { get; set; }

        /// <summary>Omschrijving van de vluchtcode</summary>
        public string FlightCodeDescription { get; set; }

        /// <summary>IATA-code van de vlucht</summary>
        public string FlightCodeIATA { get; set; }

        /// <summary>Geeft aan of de vlucht publiek wordt aangekondigd</summary>
        public bool PublicAnnouncement { get; set; }

        /// <summary>Geplande vertrektijd in UTC</summary>
        public DateTime ScheduledUTC { get; set; }

        /// <summary>Werkelijke vertrektijd in UTC</summary>
        public DateTime ActualUTC { get; set; }

        /// <summary>Geplande lokale vertrektijd</summary>
        public DateTime ScheduledLocal { get; set; }

        /// <summary>Werkelijke lokale vertrektijd</summary>
        public DateTime ActualLocal { get; set; }

        /// <summary>Soort beweging (bijv. aankomst, vertrek)</summary>
        public string Bewegingen { get; set; }

        /// <summary>Parkeerpositie van het vliegtuig</summary>
        public string Parkeerpositie { get; set; }

        /// <summary>Geeft aan of busvervoer gebruikt wordt</summary>
        public bool Bus { get; set; }

        /// <summary>Gate-nummer</summary>
        public string Gate { get; set; }

        /// <summary>Nummer van de bagageband</summary>
        public int Bagageband { get; set; }

        /// <summary>ICAO-code van de luchthaven</summary>
        public string AirportICAO { get; set; }

        /// <summary>Naam van de luchthaven</summary>
        public string Airport { get; set; }

        /// <summary>Land van bestemming of vertrek</summary>
        public string Country { get; set; }

        /// <summary>ICAO-code van tussenstop (indien van toepassing)</summary>
        public string? ViaAirportICAO { get; set; }

        /// <summary>Naam van tussenstopluchthaven (indien van toepassing)</summary>
        public string? ViaAirport { get; set; }

        /// <summary>Registratiecode van het vliegtuig</summary>
        public string AircraftRegistration { get; set; }

        /// <summary>Aantal zitplaatsen aan boord</summary>
        public int Seats { get; set; }

        /// <summary>Maximum takeoff weight (MTOW) van het vliegtuig</summary>
        public int MTOW { get; set; }

        /// <summary>Type vliegtuig (bijv. A320)</summary>
        public string AircraftType { get; set; }

        /// <summary>Omschrijving van het vliegtuigtype</summary>
        public string AircraftDescription { get; set; }

        /// <summary>Geeft aan of het een EU-vlucht is</summary>
        public bool EU { get; set; }

        /// <summary>Geeft aan of het een Schengen-vlucht is</summary>
        public bool Schengen { get; set; }

        /// <summary>Volledige naam van de luchtvaartmaatschappij</summary>
        public string AirlineFullname { get; set; }

        /// <summary>Korte naam van de airline</summary>
        public string AirlineShortname { get; set; }

        /// <summary>ICAO-code van de luchtvaartmaatschappij</summary>
        public string AirlineICAO { get; set; }

        /// <summary>IATA-code van de luchtvaartmaatschappij</summary>
        public string AirlineIATA { get; set; }

        /// <summary>Naam van de afhandelaar (debiteur)</summary>
        public string Debiteur { get; set; }

        /// <summary>Nummer van de debiteur</summary>
        public int DebiteurNr { get; set; }

        /// <summary>Aantal mannelijke passagiers</summary>
        public int PaxMale { get; set; }

        /// <summary>Aantal vrouwelijke passagiers</summary>
        public int PaxFemale { get; set; }

        /// <summary>Aantal kinderen</summary>
        public int PaxChild { get; set; }

        /// <summary>Aantal baby’s (infants)</summary>
        public int PaxInfant { get; set; }

        /// <summary>Aantal mannelijke transitpassagiers</summary>
        public int PaxTransitMale { get; set; }

        /// <summary>Aantal vrouwelijke transitpassagiers</summary>
        public int PaxTransitFemale { get; set; }

        /// <summary>Aantal transitkinderen</summary>
        public int PaxTransitChild { get; set; }

        /// <summary>Aantal transitbaby’s</summary>
        public int PaxTransitInfant { get; set; }

        /// <summary>Aantal cabinecrewleden</summary>
        public int CrewCabin { get; set; }

        /// <summary>Aantal cockpitcrewleden</summary>
        public int CrewCockpit { get; set; }

        /// <summary>Gewicht van bagage in kilogram</summary>
        public double BagsWeight { get; set; }

        /// <summary>Gewicht van transitbagage in kilogram</summary>
        public double BagsTransitWeight { get; set; }

        /// <summary>Totaal aantal bagagestukken</summary>
        public int Bags { get; set; }

        /// <summary>Aantal transitbagagestukken</summary>
        public int BagsTransit { get; set; }

        /// <summary>Afhandelaar van de vlucht</summary>
        public string Afhandelaar { get; set; }

        /// <summary>Verwacht percentage bezetting</summary>
        public double ForecastPercentage { get; set; }

        /// <summary>Verwacht aantal passagiers</summary>
        public int ForecastPax { get; set; }

        /// <summary>Verwacht aantal baby’s</summary>
        public int ForecastBabys { get; set; }

        /// <summary>Klasindeling van de vlucht (economy, business, etc.)</summary>
        public string FlightClass { get; set; }

        /// <summary>Bron van de data (bijv. real-time feed)</summary>
        public string Datasource { get; set; }

        /// <summary>Totaal aantal passagiers</summary>
        public int TotalPax { get; set; }

        /// <summary>Passagiers in de terminal</summary>
        public int TerminalPax { get; set; }

        /// <summary>Totaal aantal betalende passagiers</summary>
        public int TotalPaxBetalend { get; set; }

        /// <summary>Betalende passagiers in de terminal</summary>
        public int TerminalPaxBetalend { get; set; }

        /// <summary>Aantal transitpassagiers</summary>
        public int TransitPax { get; set; }

        /// <summary>Betalende transitpassagiers</summary>
        public int TransitPaxBetalend { get; set; }

        /// <summary>Totaal aantal crewleden</summary>
        public int TotalCrew { get; set; }

        /// <summary>Crewleden in de terminal</summary>
        public int TerminalCrew { get; set; }

        /// <summary>Totaal aantal stoelen</summary>
        public int TotalSeats { get; set; }

        /// <summary>Aantal stoelen in de terminal</summary>
        public int TerminalSeats { get; set; }

        /// <summary>Totaal aantal bagagestukken</summary>
        public int TotalBags { get; set; }

        /// <summary>Bagagestukken in de terminal</summary>
        public int TerminalBags { get; set; }

        /// <summary>Transitbagagestukken</summary>
        public int TransitBags { get; set; }

        /// <summary>Totaal gewicht van alle bagage</summary>
        public double TotalBagsWeight { get; set; }

        /// <summary>Gewicht van bagage in de terminal</summary>
        public double TerminalBagsWeight { get; set; }

        /// <summary>Gewicht van transitbagage</summary>
        public double TransitBagsWeight { get; set; }

        /// <summary>Naam of ID van de gebruikte landingsbaan</summary>
        public string Runway { get; set; }

        /// <summary>Lengtegraad van het vliegveld</summary>
        public double Longitude { get; set; }

        /// <summary>Hoogte in meters boven zeeniveau</summary>
        public double Elevation { get; set; }

        /// <summary>Breedtegraad van het vliegveld</summary>
        public double Latitude { get; set; }

        /// <summary>Afstand in kilometers tot bestemming</summary>
        public double DistanceKilometers { get; set; }

        /// <summary>Windrichting of aanvliegrichting</summary>
        public string Direction { get; set; }

        /// <summary>IATA-code van de luchthaven</summary>
        public string AirportIATA { get; set; }

        /// <summary>Geeft aan of het vliegtuig geparkeerd staat</summary>
        public bool Parked { get; set; }

        /// <summary>Seizoen van de vlucht (zomer, winter)</summary>
        public string Seizoen { get; set; }

        /// <summary>Touchpoints gekoppeld aan deze vlucht</summary>
        [JsonIgnore]
        public ICollection<Touchpoint> Touchpoints { get; set; } = new List<Touchpoint>();
    }
}
