# Backend API (.NET 10)

## Endpoint REST pubblici
- `GET /api/Employee`: elenco dipendenti
- `GET /api/Employee/{id}`: dettaglio dipendente
- `POST /api/Employee`: creazione dipendente
- `PUT /api/Employee/{id}`: aggiornamento dipendente
- `DELETE /api/Employee/{id}`: eliminazione dipendente
- `GET /WeatherForecast`: endpoint demo meteo

## Endpoint GraphQL
- `POST /graphql`
- UI GraphiQL (solo Development): `/ui/graphiql`

## Query GraphQL principali
- `employees`
- `employeesPaged(page, pageSize, sortBy, sortDir, sort)`
- `employeeById(id)`
- `reviews`
- `reviewById(id)`
- `reviewsByEmployeeId(employeeId)`

## Mutation GraphQL principali
- `addEmployee(input)`
- `updateEmployee(id, input)`
- `deleteEmployee(id)`
- `addReview(input)`
- `updateReview(id, input)`
- `deleteReview(id)`

## Contratti
- OpenAPI: [`artifacts/openapi/openapi-v1.json`](../../artifacts/openapi/openapi-v1.json)
- I tipi GraphQL sono definiti in `EmployeeManagementGraphQL/GraphQL/Types`.
