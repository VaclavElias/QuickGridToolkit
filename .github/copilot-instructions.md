# Copilot for QuickGrid.Toolkit Blazor Library

These repository instructions guide GitHub Copilot to assist development in the QuickGrid.Toolkit library and its demo app.

## Project Overview

This repository contains:

- `QuickGrid.Toolkit`: a Blazor components and utilities library.
- `QuickGrid.Samples`: a demo Blazor Web App (server interactivity) showcasing QuickGrid.Toolkit.
- `BlazorApp1`: an old demo Blazor Web App

The solution follows a clean, layered architecture pattern.

## Target Frameworks

- Runtime and libraries: .NET 9

## Technologies

- Backend: .NET / ASP.NET Core
- Frontend: Blazor (server interactivity), minimal JS interop as needed

## Folder Structure

- `/src/QuickGrid.Toolkit`: Shared core functionality (components, utilities).
- `/src/QuickGrid.Samples`: Demo Blazor app using QuickGrid.Toolkit.

## Coding Style & Conventions

- Prefer the latest C# features (file-scoped namespaces, pattern matching, primary constructors where suitable).
- Async-first: use `async/await`; use CancellationToken for I/O; avoid sync-over-async.
- One public type per file.
- Avoid `#region`; write self-explanatory code.
- Domain purity: keep domain types persistence-ignorant; put EF configuration in DbContexts.

### Dependency Injection & Services

- Register new services via IServiceCollection.
- Prefer constructor injection.
- For cross-cutting concerns, consider IServiceCollection extension methods.

## Blazor UI Guidelines

- Blazor-first for new UI work.
- For complex components, use .razor + code-behind .razor.cs partial classes.
- Use QuickGrid and QuickGrid.Toolkit for data grids rather than introducing new grid libraries.
- Keep JS interop minimal; prefer native Blazor/.NET patterns.

## Key Projects (Awareness)

- `QuickGrid.Toolkit`: shared foundational logic (components/utilities).
- `QuickGrid.Samples`: demo Blazor Web App (server interactivity) using QuickGrid.Toolkit.

## External APIs

- No external APIs at the moment.

## Best Practices

- Maintain architectural boundaries (domain remains persistence-ignorant; infrastructure handles implementation).
- Validate invariants in the domain layer where possible.
- Avoid premature abstraction; wait for proven duplication.
- Favor explicitness over reflection-heavy patterns unless already established.

## Refactoring & Change Scope (Important)

When a refactor or improvement is requested:
- If you spot minor related cleanups in nearby code, list them as suggestions with rationale and effort; do not apply unasked.
- Only proceed with broader refactors after explicit confirmation.
- Keep unrelated formatting or XML doc changes out of focused outputs unless requested.
- If a suggested improvement reduces risk (e.g., obvious bug, disposal, async fix), clearly flag severity in the suggestion.

## Maintenance

> [!IMPORTANT]
> Keep this document accurate. Update when introducing new technologies, shifting architectural direction, or adopting new patterns

Maintenance checklist:
- Review and update when the solution structure changes (e.g., migration into /src).
- Add newly adopted patterns (e.g., caching strategy, messaging, background jobs).
- Remove deprecated guidance.
- Ensure examples match the current .NET/C# level.

## How Copilot Should Behave (Implicit Guidance)

- Prefer suggestions that align with current patterns over introducing new frameworks.
- Prioritize Blazor patterns over MVC or Razor Pages.
- Ask for clarification when architecture is ambiguous.

## References

- GitHub Copilot repository instructions: https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot