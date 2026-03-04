# Repository Guidelines

## Project Structure & Module Organization
This repository contains one ASP.NET Core project inside `EmployeeManagementGraphQL/` and one solution file `Employee.slnx`.

- `EmployeeManagementGraphQL/Program.cs`: app startup, middleware, GraphQL and REST wiring.
- `EmployeeManagementGraphQL/Controllers/`: HTTP controllers (currently `WeatherForecastController`).
- `EmployeeManagementGraphQL/appsettings*.json`: environment configuration.
- `EmployeeManagementGraphQL/Properties/launchSettings.json`: local run profiles and ports.
- `EmployeeManagementGraphQL/EmployeeManagementGraphQL.http`: quick API request samples.

When adding tests, place them in a sibling folder such as `tests/EmployeeManagementGraphQL.Tests/`.

## Build, Test, and Development Commands
- `dotnet restore Employee.slnx`: restore NuGet packages.
- `dotnet build Employee.slnx`: build the solution.
- `dotnet run --project EmployeeManagementGraphQL`: run locally (HTTP `http://localhost:5232`, HTTPS `https://localhost:7252`).
- `dotnet watch --project EmployeeManagementGraphQL run`: run with hot reload.
- `dotnet test`: run tests (after a test project is added).

Current status: `dotnet build` fails due missing `UseGraphiQL` extension resolution in `Program.cs`; fix this before expecting green CI.

## Coding Style & Naming Conventions
Use standard C#/.NET conventions:

- 4-space indentation, UTF-8, one public type per file.
- `PascalCase` for classes, methods, properties; `camelCase` for locals/parameters.
- Interfaces start with `I` (example: `IEmployeeRepository`).
- Async methods end with `Async`.
- Keep controllers thin; move business logic into services.

Run `dotnet format` before opening a PR.

## Testing Guidelines
Preferred stack: xUnit + FluentAssertions + NSubstitute.

- Name tests using behavior format, e.g. `GetById_WhenEmployeeExists_ReturnsOk`.
- Follow Arrange/Act/Assert.
- Cover success, validation errors, and not-found paths.
- For GraphQL endpoints, include query and mutation integration tests.

## Commit & Pull Request Guidelines
Keep commits focused and imperative. Existing history uses Italian imperative style (example: `Inizializza repository ...`); keep that tone consistent.

- Example commit: `Aggiungi query GraphQL per elenco dipendenti`.
- PRs should include: purpose, key changes, test evidence (`dotnet build`, `dotnet test` output), and linked issue/task.
- Include request/response examples when changing API behavior.

## Security & Configuration Tips
- Never commit secrets in `appsettings*.json`; use environment variables or user secrets.
- Validate all external input in controllers/resolvers.
- Keep NuGet packages updated and review GraphQL middleware exposure in Development vs Production.
