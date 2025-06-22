# ProjectD –  FlightAPI

**Backend API voor het beheren van vluchtdata en touchpoints**, ontwikkeld in ASP.NET Core met een PostgreSQL-database. Dit project is onderdeel van het HBO-ICT vak Informatica Project-D (CMI-INF2H).

## Inhoud

- [Over dit project](#over-dit-project)
- [Features](#features)
- [Technologieën](#technologieën)
- [Installatie](#installatie)
- [Gebruik](#gebruik)
- [Database structuur](#database-structuur)
- [Authenticatie](#authenticatie)
- [Projectstructuur](#projectstructuur)
- [Team](#team)

---

## Over dit project

Luchthavens verzamelen veel vlucht- en touchpointdata, maar verwerken deze vaak inefficiënt. Dit API-project biedt een oplossing door een snelle, veilige en schaalbare interface aan te bieden waarmee gebruikers vluchtdetails kunnen opvragen, filteren en beheren.

De focus lag op:

- Prestatie en schaalbaarheid
- Role-based access control
- Error-handling en logging


---

## Features

- CRUD-functies voor **vluchten** en **touchpoints**
- **JWT-authenticatie** met RBAC (Admin / User)
- **Excel-import** voor bulkdata
- **Error logging** via JSON én PostgreSQL
- **Pagination** & **filtering** op data
- **Stress-tests** uitgevoerd met DNBommer

---

## Technologieën

| Technologie      | Toelichting                                      |
|------------------|--------------------------------------------------|
| ASP.NET Core     | Framework voor RESTful API                       |
| PostgreSQL       | Relationale database                             |
| Entity Framework | ORM voor databasekoppeling                       |
| Serilog          | Logging van fouten en requests                   |
| JWT              | Beveiliging via JSON Web Tokens                  |


### Belangrijke NuGet-packages

- `Microsoft.EntityFrameworkCore`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Newtonsoft.Json`
- `Serilog`
- `Serilog.Sinks.File`

---

## Installatie

### 1. Vereisten

- .NET 8 SDK
- PostgreSQL + pgAdmin 4
- Visual Studio 2022 of VS Code
- (Optioneel) Docker

### 2. Repository clonen

```bash
git clone https://github.com/Ziyabraza/ProjectD.git
cd ProjectD
```

### 3. PostgreSQL database opzetten

- Maak een database aan genaamd `ProjectD_DB`.
- Zet de connection string in `Backend/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ProjectD_DB;Username=postgres;Password=admin"
}
```

### 4. Start de applicatie

```bash
cd Backend
dotnet run
```

De API draait standaard op: `https://localhost:5165`

---

## Gebruik

### Voorbeelden van endpoints

- `GET /api/flights` – Alle vluchten
- `GET /api/flights/{id}` – Vlucht op ID
- `GET /api/touchpoints/flight/{flightId}` – Touchpoints bij vlucht
- `POST /api/excel/upload` – Excel importeren
- `POST /api/auth/login` – JWT-token verkrijgen

---

## Authenticatie

Na inloggen via `/api/auth/login` ontvang je een JWT-token. Gebruik deze in de headers van je requests:

```
Authorization: Bearer {token}
```

Er zijn twee rollen:
- **Admin** – toegang tot alle functies
- **User** – alleen read-only toegang

---

## Projectstructuur

```
ProjectD/
│
├── Backend/
│   ├── Controllers/           // API-logica
│   ├── Models/                // Data-entiteiten
│   ├── Services/              // ExcelService en ondersteuning
│   ├── Infrastructure/        // EF DB-context + Errors
│   └── Program.cs             // Startup + configuratie
│
├── Data/                      // Gebruikers en foutlogs
├── Migrations/                // EF Core migraties
├── Frontend/ (optioneel)      // Niet in gebruik
```
## Aanbevolen projectstructuur voor testen
Om conflicten met de debugger te voorkomen en de testomgeving overzichtelijk te houden, raden we aan om een aparte Tests-map aan te maken naast de hoofdmap ProjectD. Plaats hierin de XunitTests-folder en bijbehorende bestanden. De structuur ziet er dan als volgt uit:
├── ProjectD
└── Tests
    ├── XunitTests      ← Hier de testcode (verplaatst vanuit de oorspronkelijke locatie)
    └── Tests.sln       ← Oplossingsbestand, gekopieerd of aangemaakt in deze map
Verplaats de map XunitTests naar de map Tests, zodat deze niet meer binnen ProjectD staat. Dit voorkomt debugger-conflicten tussen testproject en hoofdproject.

Zorg ervoor dat het bestand XunitTests.csproj geen extra handmatige referenties of configuratie meer nodig heeft. Door deze structuur herkent .NET de projectafhankelijkheden automatisch correct.

---

## Team

- Hudayfa Scholten
- Ziyab Raza
- Ammar Šibonjic
- Bora Gülsaçan

---

## Licentie

Geen officiële licentie toegepast. Gebruik voor educatieve doeleinden toegestaan.
