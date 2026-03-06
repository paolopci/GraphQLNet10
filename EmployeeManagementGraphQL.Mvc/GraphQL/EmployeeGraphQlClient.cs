using System.Net.Http.Json;

namespace EmployeeManagementGraphQL.Mvc.GraphQL;

public sealed class EmployeeGraphQlClient(HttpClient httpClient) : IEmployeeGraphQlClient
{
    private const string Query = """
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

    public async Task<EmployeePagedQueryResult> GetEmployeesPagedAsync(
        int page,
        int pageSize,
        string sortField,
        string sortDir,
        CancellationToken cancellationToken = default)
    {
        var request = new GraphQlRequest<EmployeesPagedVariables>
        {
            Query = Query,
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
