using System.Net.Http.Json;

namespace EmployeeManagementGraphQL.Mvc.GraphQL;

public sealed class EmployeeGraphQlClient(HttpClient httpClient) : IEmployeeGraphQlClient
{
    private const string EmployeesPagedQuery = """
        query EmployeesPaged($page: Int!, $pageSize: Int!, $sortBy: EmployeeSortField!, $sortDir: SortDirection!) {
          employeesPaged(page: $page, pageSize: $pageSize, sortBy: $sortBy, sortDir: $sortDir) {
            items {
              id
              firstName
              lastName
              email
            }
            pageInfo {
              page
              pageSize
              totalCount
              totalPages
              hasNextPage
              hasPreviousPage
            }
          }
        }
        """;

    private const string EmployeeByIdQuery = """
        query EmployeeById($id: Int!) {
          employeeById(id: $id) {
            id
            firstName
            lastName
            email
          }
        }
        """;

    private const string AddMutation = """
        mutation AddEmployee($input: EmployeeInput!) {
          addEmployee(input: $input) {
            id
            firstName
            lastName
            email
          }
        }
        """;

    private const string UpdateMutation = """
        mutation UpdateEmployee($id: Int!, $input: EmployeeInput!) {
          updateEmployee(id: $id, input: $input) {
            id
            firstName
            lastName
            email
          }
        }
        """;

    private const string DeleteMutation = """
        mutation DeleteEmployee($id: Int!) {
          deleteEmployee(id: $id)
        }
        """;

    public async Task<EmployeePagedQueryResult> GetEmployeesPagedAsync(
        int page,
        int pageSize,
        string sortField,
        string sortDir,
        CancellationToken cancellationToken = default)
    {
        var request = new GraphQlRequest<EmployeesPagedVariables>
        {
            Query = EmployeesPagedQuery,
            Variables = new EmployeesPagedVariables
            {
                Page = page,
                PageSize = pageSize,
                SortBy = ToGraphQlSortField(sortField),
                SortDir = ToGraphQlSortDirection(sortDir)
            }
        };

        GraphQlResponse<EmployeesPagedResponseData>? responseBody;

        try
        {
            using var response = await httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new EmployeePagedQueryResult
                {
                    Success = false,
                    ErrorMessage = $"Errore HTTP verso GraphQL: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<EmployeesPagedResponseData>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new EmployeePagedQueryResult
            {
                Success = false,
                ErrorMessage = $"Impossibile contattare il backend GraphQL: {ex.Message}"
            };
        }

        if (responseBody is null)
        {
            return new EmployeePagedQueryResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL non valida o non deserializzabile."
            };
        }

        if (responseBody.Errors is { Count: > 0 })
        {
            var errorMessage = string.Join(" | ", responseBody.Errors.Select(error => error.Message));
            return new EmployeePagedQueryResult
            {
                Success = false,
                ErrorMessage = $"GraphQL ha restituito errori: {errorMessage}"
            };
        }

        var payload = responseBody.Data?.EmployeesPaged;
        if (payload?.PageInfo is null)
        {
            return new EmployeePagedQueryResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL incompleta: pageInfo mancante."
            };
        }

        return new EmployeePagedQueryResult
        {
            Success = true,
            Items = payload.Items,
            PageInfo = payload.PageInfo
        };
    }

    public async Task<EmployeeQueryResult> GetEmployeeByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var request = new GraphQlRequest<EmployeeByIdVariables>
        {
            Query = EmployeeByIdQuery,
            Variables = new EmployeeByIdVariables
            {
                Id = id
            }
        };

        GraphQlResponse<EmployeeByIdResponseData>? responseBody;

        try
        {
            using var response = await httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new EmployeeQueryResult
                {
                    Success = false,
                    ErrorMessage = $"Errore HTTP verso GraphQL: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<EmployeeByIdResponseData>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new EmployeeQueryResult
            {
                Success = false,
                ErrorMessage = $"Impossibile contattare il backend GraphQL: {ex.Message}"
            };
        }

        if (responseBody is null)
        {
            return new EmployeeQueryResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL non valida o non deserializzabile."
            };
        }

        if (responseBody.Errors is { Count: > 0 })
        {
            var errorMessage = string.Join(" | ", responseBody.Errors.Select(error => error.Message));
            return new EmployeeQueryResult
            {
                Success = false,
                ErrorMessage = $"GraphQL ha restituito errori: {errorMessage}"
            };
        }

        return new EmployeeQueryResult
        {
            Success = true,
            Employee = responseBody.Data?.EmployeeById
        };
    }

    public async Task<EmployeeMutationResult> AddEmployeeAsync(
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken = default)
    {
        var request = new GraphQlRequest<AddEmployeeVariables>
        {
            Query = AddMutation,
            Variables = new AddEmployeeVariables
            {
                Input = new EmployeeInputVariables
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                }
            }
        };

        GraphQlResponse<AddEmployeeResponseData>? responseBody;

        try
        {
            using var response = await httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new EmployeeMutationResult
                {
                    Success = false,
                    ErrorMessage = $"Errore HTTP verso GraphQL: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<AddEmployeeResponseData>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new EmployeeMutationResult
            {
                Success = false,
                ErrorMessage = $"Impossibile contattare il backend GraphQL: {ex.Message}"
            };
        }

        if (responseBody is null)
        {
            return new EmployeeMutationResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL non valida o non deserializzabile."
            };
        }

        if (responseBody.Errors is { Count: > 0 })
        {
            var errorMessage = string.Join(" | ", responseBody.Errors.Select(error => error.Message));
            return new EmployeeMutationResult
            {
                Success = false,
                ErrorMessage = $"GraphQL ha restituito errori: {errorMessage}"
            };
        }

        var employee = responseBody.Data?.AddEmployee;
        return new EmployeeMutationResult
        {
            Success = employee is not null,
            ErrorMessage = employee is null ? "Add non eseguita." : null,
            Employee = employee
        };
    }

    public async Task<EmployeeMutationResult> UpdateEmployeeAsync(
        int id,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken = default)
    {
        var request = new GraphQlRequest<UpdateEmployeeVariables>
        {
            Query = UpdateMutation,
            Variables = new UpdateEmployeeVariables
            {
                Id = id,
                Input = new EmployeeInputVariables
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                }
            }
        };

        GraphQlResponse<UpdateEmployeeResponseData>? responseBody;

        try
        {
            using var response = await httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new EmployeeMutationResult
                {
                    Success = false,
                    ErrorMessage = $"Errore HTTP verso GraphQL: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<UpdateEmployeeResponseData>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new EmployeeMutationResult
            {
                Success = false,
                ErrorMessage = $"Impossibile contattare il backend GraphQL: {ex.Message}"
            };
        }

        if (responseBody is null)
        {
            return new EmployeeMutationResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL non valida o non deserializzabile."
            };
        }

        if (responseBody.Errors is { Count: > 0 })
        {
            var errorMessage = string.Join(" | ", responseBody.Errors.Select(error => error.Message));
            return new EmployeeMutationResult
            {
                Success = false,
                ErrorMessage = $"GraphQL ha restituito errori: {errorMessage}"
            };
        }

        var employee = responseBody.Data?.UpdateEmployee;
        return new EmployeeMutationResult
        {
            Success = employee is not null,
            ErrorMessage = employee is null ? "Update non eseguita: dipendente non trovato." : null,
            Employee = employee
        };
    }

    public async Task<GraphQlOperationResult> DeleteEmployeeAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var request = new GraphQlRequest<DeleteEmployeeVariables>
        {
            Query = DeleteMutation,
            Variables = new DeleteEmployeeVariables
            {
                Id = id
            }
        };

        GraphQlResponse<DeleteEmployeeResponseData>? responseBody;

        try
        {
            using var response = await httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new GraphQlOperationResult
                {
                    Success = false,
                    ErrorMessage = $"Errore HTTP verso GraphQL: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<DeleteEmployeeResponseData>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new GraphQlOperationResult
            {
                Success = false,
                ErrorMessage = $"Impossibile contattare il backend GraphQL: {ex.Message}"
            };
        }

        if (responseBody is null)
        {
            return new GraphQlOperationResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL non valida o non deserializzabile."
            };
        }

        if (responseBody.Errors is { Count: > 0 })
        {
            var errorMessage = string.Join(" | ", responseBody.Errors.Select(error => error.Message));
            return new GraphQlOperationResult
            {
                Success = false,
                ErrorMessage = $"GraphQL ha restituito errori: {errorMessage}"
            };
        }

        return new GraphQlOperationResult
        {
            Success = responseBody.Data?.DeleteEmployee ?? false,
            ErrorMessage = responseBody.Data?.DeleteEmployee == true
                ? null
                : "Delete non eseguita: dipendente non trovato."
        };
    }

    private static string ToGraphQlSortField(string sortField)
    {
        return sortField.Trim().ToLowerInvariant() switch
        {
            "firstname" => "FIRST_NAME",
            "lastname" => "LAST_NAME",
            "email" => "EMAIL",
            _ => "ID"
        };
    }

    private static string ToGraphQlSortDirection(string sortDir)
    {
        return sortDir.Trim().ToLowerInvariant() switch
        {
            "desc" => "DESC",
            _ => "ASC"
        };
    }
}
