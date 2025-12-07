# CipherStore 🛡️

![.NET](https://img.shields.io/badge/.NET-8.0-purple) ![React](https://img.shields.io/badge/React-18-blue) ![Build Status](https://img.shields.io/badge/build-passing-brightgreen) ![Tests](https://img.shields.io/badge/tests-100%25-success)

**CipherStore** är en fullstack e-handelsapplikation utvecklad för att sälja säkerhets- och integritetshårdvara. Projektet är byggt med modern webbteknik och följer principen om **Clean Architecture**.

Denna applikation uppfyller kraven för **Väl Godkänt (VG)** genom avancerad arkitektur, omfattande testning, CI/CD-pipelines och reflekterande designval.

---

## 🏗️ Arkitekturoversikt

Projektet är strukturerat enligt **Clean Architecture** för att säkerställa "Separation of Concerns", testbarhet och underhållbarhet.

### Backend (.NET 8 API)
Backend är uppdelad i fyra lager:
1.  **Domain:** Kärnan i applikationen. Innehåller entiteter (`Product`, `Order`, `OrderItem`) och Enums. Detta lager har inga beroenden.
2.  **Application:** Innehåller affärslogik, Interfaces (t.ex. `IProductService`), DTOs och AutoMapper-profiler. Här sker validering och logikhantering.
3.  **Infrastructure:** Implementerar interfaces för databasåtkomst (`Repositories`) och externa tjänster (E-post). Här bor `AppDbContext` (EF Core).
4.  **API:** Startprojektet. Innehåller Controllers och hanterar HTTP-requests/responses samt Global Exception Handling.

### Frontend (React)
Byggd med **Vite** och **TailwindCSS**.
* **State Management:** Använder Context API för varukorgshantering.
* **API-integration:** Axios används för kommunikation med backend.
* **Struktur:** Uppdelad i `pages`, `components`, `context` och `api` för tydlighet.

---

## 🚀 Funktioner & VG-Leveranser

### Backend
* ✅ **CRUD-operationer:** Fullständig hantering av produkter och ordrar.
* ✅ **Relationer:** One-to-Many (Order -> OrderItems).
* ✅ **Global Error Handling:** Middleware som fångar och standardiserar felmeddelanden.
* 🏅 **Server-side Filtrering (VG):** Effektiv filtrering av produkter via query parameters direkt mot databasen.
* 🏅 **Lagerhantering:** Logik som automatiskt drar av lagersaldo vid köp.

### Frontend
* ✅ **Responsiv Design:** Listvyer och detaljvyer för produkter.
* ✅ **Formulärvalidering:** Validering vid checkout och inloggning.
* 🏅 **Admin Dashboard (VG):** Gränssnitt för att se statistik, ändra orderstatus och manuellt uppdatera lagersaldo.

### DevOps & Kvalitet
* 🏅 **CI/CD:** GitHub Actions workflow som automatiskt bygger och testar koden vid varje push.
* 🏅 **Testning:** 12+ tester (både Enhetstester och Integrationstester).

---

## ⚙️ Instruktioner för att starta projektet

### Förutsättningar
* .NET 8 SDK
* Node.js & npm
* SQL Server (Lokalt eller Docker)

### 1. Starta Backend
**OBS:** Projektet `API` är "Startup Project".

1.  Gå till API-mappen:
    ```bash
    cd CipherStore/API
    ```
2.  (Vid första körning) Uppdatera databasen:
    *Kontrollera connection string i `appsettings.json` först.*
    ```bash
    dotnet ef database update --project ../Infrastructure --startup-project .
    ```
3.  Starta servern:
    ```bash
    dotnet run
    ```
    *Servern startar på `https://localhost:7091`.*

### 2. Starta Frontend
1.  Gå till roten av frontend (där `package.json` finns):
    ```bash
    cd CipherStore
    ```
2.  Installera beroenden:
    ```bash
    npm install
    ```
3.  Starta applikationen:
    ```bash
    npm run dev
    ```
    *Appen nås via `http://localhost:5173`.*

### 3. Kör Tester
För att verifiera att alla enhetstester och integrationstester går grönt:
```bash
dotnet test
