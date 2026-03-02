# CLAUDE.md — Komfort24 Codebase Guide

## Project Overview

**Komfort24** is a CRM system for a Ukrainian service company (Komfort-Odessa) that manages utility billing for apartment buildings. The defining architectural choice is that **Google Sheets serves as the entire data store** instead of a traditional database.

The solution contains two user-facing applications:
- **Web** — ASP.NET Core 3.1 + Angular 10 SPA for tenants to look up their utility bill summaries
- **Desktop** — WPF application (Windows only) for company staff to generate flyers, print payment history, and perform month-end transitions

---

## Solution Structure

```
KomfortApps.sln
├── GoogleLib/        — Google Sheets/Drive API wrapper
├── SheetsEF/         — Custom "ORM" abstraction over Google Sheets (caching + scheduled sync)
├── Models/           — Shared domain models
├── Tools/            — Utilities (date helpers, validators, extensions)
├── Web/              — ASP.NET Core backend + Angular 10 SPA
│   └── ClientApp/    — Angular frontend
└── Desktop/          — WPF desktop app (Windows only)
```

All six `.csproj` files target **netcoreapp3.1**. The Desktop project also sets `<UseWPF>true</UseWPF>` and adds a COM reference to Microsoft Word for document generation.

---

## Technology Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 3.1 (C#) |
| Frontend | Angular 10, TypeScript 3.9, Bootstrap 4, RxJS 6.5 |
| Data store | Google Sheets (via Google.Apis.Sheets.v4 1.48) |
| File storage | Google Drive (via Google.Apis.Drive.v3 1.48) |
| Scheduling | Quartz.NET 3.2.3 |
| Caching | Microsoft.Extensions.Caching.Memory |
| Desktop UI | WPF (.NET Core 3.1, Windows only) |
| Document gen | Microsoft Office Word COM Interop |
| Containerisation | Docker (multi-stage, Node 14 + .NET 3.1) |

---

## Domain Model

The company manages **7 apartment buildings** at fixed addresses on Пишоновская street in Odessa:

```
20/1 | 24/2 | 22/2 | 26/2 | 26/1 | 20/2 | 24A
```

Each house contains up to **120 accounts** (flats), stored in Google Sheets in sequential blocks (`MaxAccountNumber = 120`). The order of addresses in `ServiceContext.InitContextAsync` must match the row order in the Sheets data.

### Core Classes

| Class | Location | Purpose |
|---|---|---|
| `ServiceContext` | `Models/` | Aggregates all houses; loaded from Google Sheets |
| `House` | `Models/` | One building; holds `List<Account>` and `Rates` |
| `Account` | `Models/` | One flat; holds identity info and `CurrentReport` |
| `Rates` | `Models/` | Per-house utility tariffs |
| `Report2` | `Models/` | Current report structure (2022+) |
| `Report` | `Models/` | Legacy report structure (pre-2022); still used for archive queries |
| `ArchiveReport2` | `Models/` | `Report2` + identity fields + `Month`/`Year` |
| `ArchiveReport` | `Models/` | `Report` + identity fields (legacy archive) |

### Account ID Format

Account IDs are validated by `Tools.Matching.IsAccountId()` using the regex `(\d{4}$)|(\d{4}/[1|2]$)` — either exactly 4 digits or 4 digits followed by `/1` or `/2`.

Account with ID **"7695"** is explicitly skipped in archive operations (`HouseExtensions.GetObjects`).

### Heating Types

Valid heating type strings (stored in Google Sheets, compared case-insensitively):

| String | Unit |
|---|---|
| `"гкал"` | Gcal |
| `"мвт"` | MWt (coefficient 0.86 × 1.1) |
| `"квт"` | KWt (÷ 1162.2) |
| `"гдж"` | GJ (÷ 4.187) |
| `""` | Central heating (flat rate) |

Any other value will throw `ArgumentException` during month transition and flyer generation.

### Report2 vs Report (Column Offsets)

`Report2` maps `IList<object>` from column index 3 onward (columns 0–2 are flat/account/owner). `Report` has a slightly different layout (fewer fields — no `RepairForMonth`, `LivingPersons`, `GarbageForMonth`, `WerRepair`). When reading legacy archives, `ArchiveReport` is constructed from the old layout and then converted to `ArchiveReport2` via its copy constructor.

---

## Google Sheets Data Sources

Spreadsheet IDs are hardcoded in `GoogleLib/Sheets.cs`:

| Constant | Purpose |
|---|---|
| `ServiceSpreadSheetId` | `Houses!A1:AK1000` (current accounts + rates) |
| `HeatingSpreadSheetId` | Per-house heating ledger sheets |
| `WerSpreadSheetId` | Per-house utilities ledger sheets |
| `ReportsSpreadSheetId` | Legacy archive (`Info!A1:AH250000`) |
| `ReportsSpreadSheetId2` | Current archive (`Info!A1:AL250000`) |

Sheet names containing Cyrillic (e.g., `"Сводная ведомость"`, `"Houses"`, `"Rates"`) must match exactly what exists in Google Sheets.

---

## Authentication

Both the Web and Desktop apps require a **`client_secret.json`** file in the working directory at runtime. This file contains Google OAuth 2.0 credentials and is copied to the build output directory (`PreserveNewest`). The file is git-ignored — you must obtain it separately.

After first run, the OAuth token is cached to an `auth/` subdirectory (also git-ignored). The Desktop app requests `Drive.Scope.Drive` + `Sheets.Scope.Spreadsheets`; the Web app requests `Sheets.Scope.SpreadsheetsReadonly`.

---

## SheetsEF — The Caching Layer

`SheetsEF` is a lightweight custom abstraction that mimics Entity Framework's `DbContext` pattern over Google Sheets.

- `ApplicationContextBase` holds an `IMemoryCache` with **10-minute** TTL entries.
- Subclasses (e.g., `Web.Models.ApplicationContext`) declare typed properties that map to cache keys:
  ```csharp
  public ServiceContext Service => GetData<ServiceContext>(nameof(Service));
  ```
- A **Quartz.NET** background job (`UpdateSheetsJob<T>`) calls `UpdatingSheetsAsync()` on the configured cron schedule. The Web app uses `"0 0/5 * ? * * *"` (every 5 minutes).
- Registration via DI extension: `services.AddSheetsContext<ApplicationContext>("cron-expression")`.

---

## Web Application

### Running Locally

**Prerequisites:** .NET Core 3.1 SDK, Node.js ≥ 14, `client_secret.json` in `Web/`.

```bash
# Install Angular dependencies first
cd Web/ClientApp
npm install
cd ../..

# Run backend (Angular dev server starts automatically via AngularCli middleware)
dotnet run --project Web/Web.csproj
```

The Angular dev server is proxied through the ASP.NET Core host on port 5000/5001 (see `launchSettings.json`).

### Publishing (Production)

```bash
dotnet publish Web/Web.csproj -c Release -o ./publish
```

The `PublishRunWebpack` MSBuild target automatically runs `npm install && npm run build --prod` before publishing.

### Docker

```bash
docker build -f Web/Dockerfile -t komfort24-web .
docker run -p 8080:80 komfort24-web
```

The Dockerfile is a multi-stage build. The SDK stage installs Node.js 14 (`curl ... nodesource_setup.sh`), builds Angular, then publishes the .NET project.

### REST API

There is currently **one API controller**:

```
GET /api/account?accountId={id}   →  Account (with House, Rates, Report2)
```

Returns `null` if account not found. The Angular `DataService` maps the raw response and computes derived fields (start states, paid amounts, end states) client-side.

---

## Desktop Application

The WPF app targets **Windows only** and requires Microsoft Word to be installed (it uses COM Interop via `Microsoft.Office.Interop.Word`).

**Required files at runtime:**
- `client_secret.json`
- `Resources/FlyerTemplate.docx` — Word template for monthly flyers

### MVVM Structure

```
AppViewModel
├── GenerationViewModel   → GenerationFlyersCommand
├── TransitionViewModel   → TransitionToNewMonthCommand
└── SearchViewModel       → SearchReportsCommand + PrintReportsCommand
```

`RelayCommand` implements `ICommand` using `CommandManager.RequerySuggested` for `CanExecuteChanged`. Call `RelayCommand.RaiseCanExecuteChanged()` after async operations to force UI refresh.

### Operations

#### 1. Flyer Generation (`GenerationFlyersCommand`)

Reads current data from Google Sheets, creates a `.docx` file per house using `FlyerTemplate.docx` as a template, and populates Word `Find & Replace` placeholders (`{NM}`, `{AD}`, `{HSS}`, etc.). Files are saved to `Квитанции за {currentMonth}/` in the working directory. Supports cancellation via `CancellationTokenSource`.

#### 2. Month Transition (`TransitionToNewMonthCommand`)

1. Checks Google Drive for a folder named `{prevMonth} {year}` — aborts if already exists (prevents double-transition)
2. Validates all heating types in current data
3. Creates a Google Drive folder for the previous month and copies spreadsheet files into it
4. Appends current month's data to the archive sheet (`ReportsSpreadSheetId2`)
5. Resets per-account fields for the new month (start = previous end, payments cleared)
6. Updates the pivot table month/year cells

#### 3. Search & Print (`SearchReportsCommand`, `PrintReportsCommand`)

Fetches full archive data (both `ReportsSpreadSheetId` and `ReportsSpreadSheetId2`) on first search; cached for the session. Converts legacy `ArchiveReport` to `ArchiveReport2` for unified display. Prints via a dynamically constructed Word table (max 36 records).

---

## Angular Frontend

### Component Tree

```
AppComponent (root)
├── MainPageComponent    — Landing page with scroll-to-service button
├── ServicePageComponent — Account search; calls DataService; displays report
└── AppFooterComponent   — Footer
```

No Angular Router guards or lazy loading — all routes are eager.

### Utility Calculations (ServicePageComponent)

The backend returns raw debit/credit/value fields. The Angular component re-computes display values locally using `updateTotal()` on every field change. This mirrors the flyer generation logic. When adding new service types, both the `Report2Extensions.GetFormula` C# method and `ServicePageComponent.updateTotal()` TypeScript method must stay in sync.

### Heating Coefficient Conversion

```typescript
// In ServicePageComponent
"гкал" → coef = 1.1
"мвт"  → coef = 0.86 * 1.1
"квт"  → coef = 1.1 / 1162.2
"гдж"  → coef = 1.1 / 4.187
""     → uses centralHeatingRate (flat-rate, no meter)
```

---

## Key Utilities

### `Tools.Date`

Russian month name ↔ number conversion. Month names are lowercase Ukrainian/Russian (`"январь"` – `"декабрь"`). Use `GetNameCurMonth()` / `GetNamePrevMonth()` for display; `GetNumMonth()` when parsing archive data.

### `Tools.Extensions.ObjectExtensions`

`ToDouble(digits)` and `ToInt()` extension methods on `object`. They parse using `CultureInfo("ru-RU")` — **comma is the decimal separator**. Strip whitespace before parsing. Default precision for `ToDouble()` is 2 decimal places; heating values use 3.

### `Tools.Matching.IsAccountId`

Single static method. Called before any Google Sheets lookup to guard against invalid input.

---

## Code Conventions

- **C# naming:** PascalCase for public members and types; `_camelCase` for private fields.
- **Async:** All Google API calls are `async Task`; Desktop commands run Google calls and dispatch to background threads via `Task.Run` for CPU work.
- **Static command classes:** Desktop commands (`GenerationFlyersCommand`, etc.) are `internal static` classes with `Processing` bool guards to prevent concurrent execution.
- **Extension methods:** Domain operations that don't belong in models are placed in `Desktop/Extensions/` (e.g., `HouseExtensions`, `Report2Extensions`, `AccountExtensions`). These are `internal static` and not part of the shared `Models` or `Tools` assemblies.
- **Google API errors:** Always caught and re-thrown as `GoogleLib.Exceptions.AccessDeniedException`. Display `HelpMessage` to the user; `ErrorCode` is `-1` (network), `-2` (Google API), `-3` (other).
- **`[JsonIgnore]`:** Applied to `House.ShortAdress`, `House.Accounts`, and `House.FlatCount` to avoid circular references and noise in the Web API response.
- **Rounding:** Financial values rounded to 2 dp; meter readings (heating) to 3 dp. Use `Math.Round` explicitly before writing back to Sheets.
- **No tests:** There are no test projects in the solution. Validate changes manually.

---

## Build Commands Reference

```bash
# Full solution build
dotnet build KomfortApps.sln

# Web app — run in development
cd Web/ClientApp && npm install && cd ..
dotnet run --project Web.csproj

# Web app — publish for production
dotnet publish Web.csproj -c Release -o ./publish

# Angular only
cd Web/ClientApp
npm install
npm run build          # development build
npm run build -- --prod  # production build

# Docker
docker build -f Web/Dockerfile -t komfort24-web .
```

---

## Important Notes for AI Assistants

1. **No database migrations.** All schema changes require manually updating the Google Sheet column structure and adjusting the `Report2` (or `Report`) constructor's index constants.

2. **Order of house addresses matters.** The hardcoded list in `ServiceContext.InitContextAsync` must match the column/row order in Google Sheets. Do not reorder addresses without also updating the sheets.

3. **Two report formats coexist.** `Report` (pre-2022) and `Report2` (2022+) differ in column count and field set. Archive queries must handle both; `ArchiveReport2(ArchiveReport)` provides backward compatibility.

4. **Russian locale everywhere.** Numeric strings from Sheets use comma as decimal separator. Month names are lowercase Russian. Do not change `CultureInfo` or month name constants without testing against real Sheets data.

5. **Desktop app is Windows-only.** WPF and `Microsoft.Office.Interop.Word` COM usage cannot run on Linux/macOS. The Web project can be containerised (Docker); the Desktop project cannot.

6. **Quartz cron syntax.** Quartz uses a 7-field cron (seconds included): `"0 0/5 * ? * * *"` = every 5 minutes. This differs from standard 5-field Unix cron.

7. **No dependency injection in Desktop.** The Desktop project does not use `IServiceCollection`; objects are newed up directly. The SheetsEF DI extension is only used by the Web project.

8. **`client_secret.json` is required at runtime but must not be committed.** It is already in `.gitignore`. Never add it to source control.
