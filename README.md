# CipherStore 🔒

En säker fullstack e-handelsapplikation fokuserad på integritetshårdvara (YubiKeys, Faraday bags, etc.).
Projektet är byggt enligt **Clean Architecture** med **.NET 8** (Backend) och **React** (Frontend).

## 🚀 Funktioner

### För Kunder
- 🛒 **Webbshop:** Bläddra bland produkter och lägg i varukorgen.
- 🔍 **Filtrering:** Server-side filtrering av produkter baserat på kategori (VG-krav).
- 💳 **Betalning:** Säker betalning via **Stripe**.
- 📦 **Orderhantering:** Realtidsuppdatering av lagersaldo vid köp.

### För Admin (CMS)
- 🔐 **Adminpanel:** Skyddad inloggning via `/admin`.
- 📋 **Orderöversikt:** Se status på alla ordrar (Ny -> Betald -> Packad -> Skickad).
- 📉 **Lagerhantering:** Administrera lagersaldo direkt i gränssnittet (VG-krav).

## 🛠 Teknikstack

- **Backend:** .NET 8 Web API, Entity Framework Core, SQL Server.
- **Frontend:** React (Vite), TailwindCSS, Context API.
- **Arkitektur:** Clean Architecture (Domain, Application, Infrastructure, API).
- **Testning:** xUnit, Moq, FluentAssertions (100% testtäckning på services & integration).
- **CI/CD:** GitHub Actions (Automatisk build & test vid push).

## ⚙️ Kom igång (Installation)

### 1. Förberedelser
Se till att du har följande installerat:
- .NET 8 SDK
- Node.js & npm
- SQL Server (Lokalt eller via Docker)

### 2. Starta Backend (API)
**OBS:** Projektet `API` är startprojektet för backend.

1. Navigera till API-mappen:
   ```bash
   cd CipherStore/API
