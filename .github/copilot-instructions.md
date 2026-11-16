# Auth Patterns Lab — Copilot Instructions

These portable engineering rules apply to this authentication and authorization
learning lab. Keep them free of personal profiles and user-specific preferences.
The project scope and learning stages are documented in
[`doc/AUTH_PATTERNS_LAB_ROADMAP.md`](../doc/AUTH_PATTERNS_LAB_ROADMAP.md).

## Engineering priorities

- Preserve stable behavior, security, data integrity, and explicit failure paths.
- Inspect the current implementation, configuration, tests, and nearest existing
  pattern before proposing or applying a change.
- Separate facts, assumptions, unknowns, constraints, and decisions.
- Prefer the smallest coherent change that solves the requirement.
- Reuse established project patterns before introducing abstractions, packages,
  tools, or new architectural boundaries.
- Do not hide errors behind broad catches, silent fallbacks, or success-shaped
  responses.
- Preserve unrelated user changes and avoid unrelated refactoring.

## Working method

- Work on one coherent vertical slice or one hardening topic at a time.
- Inspect the current implementation, configuration, tests, and nearest
  existing pattern before proposing or applying a change.
- Define the actor, expected result, invariants, rule owner, data owner,
  meaningful failure paths, transaction boundary, and cheapest decisive test.
- Compare only a few realistic implementation options and state the recommended
  smallest variant.
- Separate facts, assumptions, unknowns, constraints, and decisions.
- Implement in small, reversible checkpoints.
- Preserve unrelated user changes. Do not delete inherited functionality until
  its replacement slice and tests have been verified.
- After a change, report layer responsibilities, risks, exact validation
  evidence, and remaining gaps.

## Conversation modes

- `PLAN ONLY`: analyze facts, options, risks, and validation without editing.
- `IMPLEMENT`: make the approved, focused change and validate it.
- `REVIEW`: assess the current code or diff without expanding the scope.
- `DEBUG`: reproduce the symptom, test a hypothesis, make the smallest repair,
  and add or run a regression check.
- `TEACH-BACK`: check understanding of the flow and failure paths without
  replacing the explanation with generated code.

For explicitly educational authentication or authorization exercises, first
ask the learner to draw the flow, explain the trust boundary, or attempt the
core policy/test. Review and teach from that attempt before supplying the
central implementation. Do not make this a prerequisite for requested
boilerplate, scaffolding, UI, or unrelated work.

## Architecture

- Keep domain rules and invariants in the domain model or the owning module.
- Keep application contracts, results, focused ports, and use-case orchestration
  separate from HTTP and persistence details.
- Keep infrastructure responsible for EF Core, PostgreSQL, migrations,
  external integrations, workers, and implementations of application ports.
- Keep API responsible for binding, HTTP contracts, validation, authorization,
  and mapping application results to status codes.
- Do not return HTTP types from handlers or stores.
- Do not place domain rules in controllers.
- Prefer explicit module entry points and composition-root registration.
- Cross-module access must use an explicit port, identifier, or application
  workflow rather than reaching into another module's persistence.
- Rules enforceable by the database should also be protected by PostgreSQL
  constraints or indexes when applicable.

## Authentication and authorization

- Each incompatible trust model must use a separate host/composition root,
  configuration, and relevant tests. Do not combine them behind a global
  `AuthMode` switch.
- Configure authentication schemes and identity-provider integration at the
  host boundary. Enforce endpoint policies and access to specific resources in
  the owning API/use case.
- Trust identity only from a successfully validated principal. Never accept a
  caller's role, owner ID, or tenant ID from UI-controlled request data.
- Keep passwords, tokens, client secrets, and unnecessary personal data out of
  logs and the React demo console.
- Gate seeded accounts and demo data to Development, Demo, or Test; document a
  safe reset path.
- Do not implement cryptography, OAuth/OIDC flows, or MFA protocols from
  scratch. Do not use implicit flow or Resource Owner Password Credentials for
  new demonstrations.
- Distinguish ID tokens from access tokens. Do not assume every access token is
  a JWT.
- Document and consistently test whether an inaccessible resource returns
  `403` or `404`. CORS does not replace authorization or CSRF protection.

## Validation

- Run the narrowest decisive test first.
- Run the affected build and expand validation when contracts, DI, routing,
  persistence, migrations, concurrency, or module boundaries change.
- Use unit tests for domain rules and isolated handler behavior.
- Use API integration tests for public routes, authorization, and response
  contracts.
- Use PostgreSQL-backed tests for transactions, constraints, migrations, and
  concurrency.
- Use frontend or browser tests when the user-visible workflow changes.
- Do not claim PostgreSQL or Docker validation passed when the environment could
  not run it.
- Label unit, API integration, PostgreSQL integration, browser E2E, manual
  identity-provider sign-in, and unrun checks accurately. A test identity
  handler does not prove a real OIDC sign-in.
- Report only checks actually run, including commands and outcomes. Do not
  infer current results from old checked-in test reports.

## Documentation and safety

- Document durable architectural decisions and contract changes.
- Keep public DTOs, endpoints, validation, status codes, and example payloads
  understandable.
- Mark generated skeletons, placeholders, and intentional limitations clearly.
- Surface validation failures explicitly and preserve actionable diagnostics.
- Do not perform Git branch, commit, merge, rebase, reset, stash, pull, push, or
  tag operations without a direct request.

Backend-specific rules are in
`.github/instructions/backend.instructions.md`. Auth lab scope and planned
scenarios are in `doc/AUTH_PATTERNS_LAB_ROADMAP.md`.
