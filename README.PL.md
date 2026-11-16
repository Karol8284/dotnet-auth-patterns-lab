# Auth Patterns Lab

[![CI](https://github.com/Karol8284/dotnet-auth-patterns-lab/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Karol8284/dotnet-auth-patterns-lab/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)

Auth Patterns Lab to edukacyjne laboratorium .NET/React do porównywania
uwierzytelniania, autoryzacji i granic zaufania. Celem jest zrozumienie,
implementacja i testowanie małych przepływów — nie budowa produktu SaaS ani
własnego frameworka auth.

**English documentation:** [README.md](README.md)

## Status projektu

Repozytorium powstało z `dotnet-react-starter` i nadal zawiera jego rozbudowaną
aplikację. Na dziś jest to etap przejściowy, a nie gotowa demo console:

- istniejący custom JWT i rotacja refresh tokenów są pierwszym materiałem do
  prześledzenia i przetestowania; nie są rekomendacją własnego issuera dla
  nowej aplikacji;
- odziedziczony frontend zapisuje access token w `localStorage`; to zachowanie
  jest analizowane, a nie wzorzec do kopiowania do docelowego BFF;
- Identity + cookie, MSAL/PKCE, BFF + zewnętrzny OIDC i service identity nie
  są jeszcze osobnymi działającymi demonstratorami.

Aktualny zakres i kolejność prac opisuje
[roadmapa Auth Patterns Lab](doc/AUTH_PATTERNS_LAB_ROADMAP.md). Nie deklarujemy
produkcyjnej gotowości na podstawie samego builda ani obecności funkcji.

## Kierunek architektury

Docelowo małe, wspólne demo domenowe (np. prywatne rekordy przypisane
użytkownikom) będzie obsługiwane przez osobne demonstratory:

```text
React demo console
  |-- Identity + cookie (osobny host)
  |-- MSAL + PKCE -> Bearer API (osobny public SPA/API wariant)
  |-- BFF + zewnętrzny OIDC (osobny host)
  `-- service/workload identity (osobny wariant)
```

Schemat auth konfiguruje host/composition root, a polityki i dostęp do
konkretnych rekordów egzekwuje backend. Nie będzie jednego globalnego
`AuthMode`, wyboru roli przez UI ani wyświetlania access/refresh tokenów.
Przykładowe konta i dane mogą działać wyłącznie w Development/Demo/Test i muszą
mieć bezpieczną ścieżkę resetu.

## Pierwsze kroki

Wymagany jest .NET 10 SDK. Testy PostgreSQL/Testcontainers i pełny lokalny stack
wymagają działającego Dockera. Frontend używa Node.js 24 LTS.

```powershell
dotnet restore backend\backend.slnx
dotnet build backend\backend.slnx --configuration Release
dotnet test backend\UnitTests\UnitTests.csproj --configuration Release
```

Testy PostgreSQL oraz integracyjne uruchamiaj według
[instrukcji backendu](doc/BACKEND_SETUP.md). Testy frontendu:

```powershell
Set-Location frontend
npm ci
npm run test:once
npm run build
```

Lokalny stack Docker Compose i konfiguracja środowiska są opisane w
[Getting Started](doc/GETTING_STARTED.md). Przykładowa konfiguracja nie jest
wdrożeniem produkcyjnym. Nie uruchamiaj testów smoke przeciwko środowisku ani
danym, do których nie masz upoważnienia.

## Zasady bezpieczeństwa i dowodów

- Nie używaj prawdziwych danych, haseł ani sekretów; nie commituj wartości
  `.env` ani tokenów.
- Nie implementuj własnej kryptografii, OAuth/OIDC flow ani MFA.
- Rozróżniaj ID token i access token; klient nie może zakładać, że access token
  musi być JWT.
- `ownerId`, `tenantId` i role z requestu nie są zaufane. Tożsamość pochodzi z
  zweryfikowanego principal, a dostęp do zasobu ustala backend.
- CORS nie zastępuje autoryzacji ani ochrony CSRF.
- Oznaczaj osobno testy jednostkowe, integracyjne API, PostgreSQL, browser E2E,
  ręczny sign-in do testowego IdP i testy nieuruchomione. Mock identity nie
  dowodzi prawdziwego sign-in OIDC.
- Zapisuj tylko faktycznie wykonane komendy i wyniki. Zachowuj diagramy,
  threat model, decyzje, ograniczenia i autorstwo kodu wspomaganego przez AI;
  są potrzebne do uczciwej oceny projektu i ewentualnej pracy naukowej.

## Dokumentacja

| Temat | Dokument |
| --- | --- |
| Zakres, etapy nauki, architektura i wymagane dowody | [AUTH_PATTERNS_LAB_ROADMAP.md](doc/AUTH_PATTERNS_LAB_ROADMAP.md) |
| Obecny JWT i rotacja refresh tokenów | [JWT_ARCHITECTURE.md](doc/JWT_ARCHITECTURE.md) |
| Konfiguracja i uruchomienie backendu | [BACKEND_SETUP.md](doc/BACKEND_SETUP.md) |
| Lokalny start aplikacji | [GETTING_STARTED.md](doc/GETTING_STARTED.md) |
| Zasady pracy z AI w repozytorium | [.github/copilot-instructions.md](.github/copilot-instructions.md) |
| Zgłaszanie podatności | [SECURITY.md](SECURITY.md) |

## Licencja

MIT. Szczegóły znajdują się w [LICENSE](LICENSE).
