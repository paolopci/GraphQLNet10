# Frontend (ASP.NET Core MVC)

Nota: nel repository non sono presenti app Angular o Blazor. Il frontend attivo e documentato e` MVC server-side in `EmployeeManagementGraphQL.Mvc`.

## Controller UI principali
- `EmployeesController`: listing paginato/sortato, create/edit/delete dipendenti
- `ReviewsController`: listing/create/edit/delete review con filtro `employeeId`
- `HomeController`: pagine base

## Integrazione con backend
- Il frontend usa `IEmployeeGraphQlClient` + `EmployeeGraphQlClient`.
- Tutte le operazioni CRUD UI passano da chiamate GraphQL su `/graphql`.
- Base URL GraphQL configurabile in `GraphQl:BaseUrl` (fallback locale `http://localhost:5232`).

## View e modelli
- Views: `Views/Employees/*`, `Views/Reviews/*`, `Views/Home/*`
- ViewModel: `Models/EmployeesIndexVm`, `EmployeeUpsertVm`, `ReviewUpsertVm`, `ReviewsIndexVm`

## Routing MVC
- Route default: `{controller=Employees}/{action=Index}/{id?}`
- Anti-forgery applicato sulle azioni POST con `[ValidateAntiForgeryToken]`.
