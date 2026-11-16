# Auth Patterns Lab

[![CI](https://github.com/Karol8284/dotnet-auth-patterns-lab/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Karol8284/dotnet-auth-patterns-lab/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)

Auth Patterns Lab is an educational .NET/React lab for comparing
authentication, authorization, and trust boundaries. Its goal is to understand,
implement, and test small flows—not to build a SaaS product or a custom
authentication framework.

**Polish documentation:** [README.PL.md](README.PL.md)

## Project status

This repository was created from `dotnet-react-starter` and still contains its
large application. It is currently in transition, not yet a finished demo
console:

- the existing custom JWT and refresh-token rotation are the first study and
  testing subject; they are not a recommendation to build a new issuer;
- the inherited frontend stores access tokens in `localStorage`; this behavior
  is being examined, not proposed as the target BFF pattern;
- Identity + cookies, MSAL/PKCE, BFF + external OIDC, and service identity are
  not yet separate working demonstrators.

See the [Auth Patterns Lab roadmap](doc/AUTH_PATTERNS_LAB_ROADMAP.md) for the
current scope and order of work. A successful build or the presence of a feature
does not establish production readiness.

## Architectural direction

The target is a small shared domain example (such as private records owned by
users) with separate demonstrators:

```text
React demo console
  |-- Identity + cookies (separate host)
  |-- MSAL + PKCE -> Bearer API (separate public SPA/API variant)
  |-- BFF + external OIDC (separate host)
  `-- service/workload identity (separate variant)
```

The host/composition root configures authentication; the backend enforces
policies and access to individual records. There will be no global `AuthMode`,
UI-controlled role selection, or display of access/refresh tokens. Sample
accounts and data may run only in Development/Demo/Test and need a safe reset
path.

## Getting started

Requires the .NET 10 SDK. PostgreSQL/Testcontainers tests and the complete local
stack require Docker. The frontend uses Node.js 24 LTS.

```powershell
dotnet restore backend\backend.slnx
dotnet build backend\backend.slnx --configuration Release
dotnet test backend\UnitTests\UnitTests.csproj --configuration Release
```

Run PostgreSQL and integration tests as described in the
[backend setup guide](doc/BACKEND_SETUP.md). Frontend tests:

```powershell
Set-Location frontend
npm ci
npm run test:once
npm run build
```

See [Getting Started](doc/GETTING_STARTED.md) for the local Compose stack and
environment setup. Example configuration is not production deployment
guidance. Do not run smoke tests against environments or data you are not
authorized to use.

## Security and evidence principles

- Do not use real data, passwords, or secrets; do not commit `.env` values or
  tokens.
- Do not implement cryptography, OAuth/OIDC flows, or MFA from scratch.
- Distinguish ID tokens from access tokens; a client must not assume every
  access token is a JWT.
- Request-supplied `ownerId`, `tenantId`, and roles are untrusted. Identity
  comes from a validated principal, and the backend decides resource access.
- CORS does not replace authorization or CSRF protection.
- Label unit, API integration, PostgreSQL, browser E2E, manual test-IdP sign-in,
  and unrun checks separately. Mock identity does not prove a real OIDC sign-in.
- Record only commands and outcomes that actually occurred. Preserve diagrams,
  threat models, decisions, limitations, and attribution of AI-assisted code;
  they matter for honest evaluation and possible academic work.

## Documentation

| Topic | Document |
| --- | --- |
| Scope, learning stages, architecture, and required evidence | [AUTH_PATTERNS_LAB_ROADMAP.md](doc/AUTH_PATTERNS_LAB_ROADMAP.md) |
| Existing JWT and refresh-token rotation | [JWT_ARCHITECTURE.md](doc/JWT_ARCHITECTURE.md) |
| Backend setup and configuration | [BACKEND_SETUP.md](doc/BACKEND_SETUP.md) |
| Local application setup | [GETTING_STARTED.md](doc/GETTING_STARTED.md) |
| Repository AI workflow | [.github/copilot-instructions.md](.github/copilot-instructions.md) |
| Vulnerability reporting | [SECURITY.md](SECURITY.md) |

## License

MIT. See [LICENSE](LICENSE).
