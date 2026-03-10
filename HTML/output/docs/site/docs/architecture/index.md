# Architettura

## Contesto
La solution contiene due applicazioni .NET 10:
- `EmployeeManagementGraphQL`: backend API REST + GraphQL + EF Core SQL Server
- `EmployeeManagementGraphQL.Mvc`: frontend MVC che consuma il backend via GraphQL client

## Layer backend
- `Controllers`: endpoint REST
- `GraphQL/Queries` e `GraphQL/Mutations`: contratto GraphQL
- `Data/Repositories`: accesso ai dati e logica CRUD
- `Data/EntityDatabaseContext`: persistenza EF Core

## Layer frontend
- `Controllers MVC`: orchestrano richieste utente
- `GraphQL client`: richieste `POST /graphql` e mapping errori
- `Views + ViewModel`: rendering e validazione lato UI

## Flusso principale
1. Browser invia richiesta al frontend MVC.
2. Controller MVC usa `EmployeeGraphQlClient`.
3. Il client invoca backend GraphQL.
4. Backend risolve query/mutation su repository/EF Core.
5. Il frontend renderizza il risultato in view Razor.
