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

    private const string ReviewsQuery = """
        query Reviews {
          reviews {
            id
            rate
            comment
            employeeId
          }
        }
        """;

    private const string ReviewsByEmployeeIdQuery = """
        query ReviewsByEmployeeId($employeeId: Int!) {
          reviewsByEmployeeId(employeeId: $employeeId) {
            id
            rate
            comment
            employeeId
          }
        }
        """;

    private const string ReviewByIdQuery = """
        query ReviewById($id: Int!) {
          reviewById(id: $id) {
            id
            rate
            comment
            employeeId
          }
        }
        """;

    private const string AddReviewMutation = """
        mutation AddReview($input: ReviewInput!) {
          addReview(input: $input) {
            id
            rate
            comment
            employeeId
          }
        }
        """;

    private const string UpdateReviewMutation = """
        mutation UpdateReview($id: Int!, $input: ReviewInput!) {
          updateReview(id: $id, input: $input) {
            id
            rate
            comment
            employeeId
          }
        }
        """;

    private const string DeleteReviewMutation = """
        mutation DeleteReview($id: Int!) {
          deleteReview(id: $id)
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

    public async Task<ReviewsQueryResult> GetReviewsAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GraphQlRequest<object>
        {
            Query = ReviewsQuery,
            Variables = new { }
        };

        GraphQlResponse<ReviewsResponseData>? responseBody;

        try
        {
            using var response = await httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ReviewsQueryResult
                {
                    Success = false,
                    ErrorMessage = $"Errore HTTP verso GraphQL: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<ReviewsResponseData>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ReviewsQueryResult
            {
                Success = false,
                ErrorMessage = $"Impossibile contattare il backend GraphQL: {ex.Message}"
            };
        }

        if (responseBody is null)
        {
            return new ReviewsQueryResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL non valida o non deserializzabile."
            };
        }

        if (responseBody.Errors is { Count: > 0 })
        {
            var errorMessage = string.Join(" | ", responseBody.Errors.Select(error => error.Message));
            return new ReviewsQueryResult
            {
                Success = false,
                ErrorMessage = $"GraphQL ha restituito errori: {errorMessage}"
            };
        }

        return new ReviewsQueryResult
        {
            Success = true,
            Reviews = responseBody.Data?.Reviews ?? []
        };
    }

    public async Task<ReviewsQueryResult> GetReviewsByEmployeeIdAsync(
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        if (employeeId <= 0)
        {
            return new ReviewsQueryResult
            {
                Success = false,
                ErrorMessage = "EmployeeId non valido."
            };
        }

        var request = new GraphQlRequest<ReviewsByEmployeeIdVariables>
        {
            Query = ReviewsByEmployeeIdQuery,
            Variables = new ReviewsByEmployeeIdVariables
            {
                EmployeeId = employeeId
            }
        };

        GraphQlResponse<ReviewsByEmployeeIdResponseData>? responseBody;

        try
        {
            using var response = await httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ReviewsQueryResult
                {
                    Success = false,
                    ErrorMessage = $"Errore HTTP verso GraphQL: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<ReviewsByEmployeeIdResponseData>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ReviewsQueryResult
            {
                Success = false,
                ErrorMessage = $"Impossibile contattare il backend GraphQL: {ex.Message}"
            };
        }

        if (responseBody is null)
        {
            return new ReviewsQueryResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL non valida o non deserializzabile."
            };
        }

        if (responseBody.Errors is { Count: > 0 })
        {
            var errorMessage = string.Join(" | ", responseBody.Errors.Select(error => error.Message));
            return new ReviewsQueryResult
            {
                Success = false,
                ErrorMessage = $"GraphQL ha restituito errori: {errorMessage}"
            };
        }

        return new ReviewsQueryResult
        {
            Success = true,
            Reviews = responseBody.Data?.ReviewsByEmployeeId ?? []
        };
    }

    public async Task<ReviewQueryResult> GetReviewByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return new ReviewQueryResult
            {
                Success = false,
                ErrorMessage = "Id review non valido."
            };
        }

        var request = new GraphQlRequest<ReviewByIdVariables>
        {
            Query = ReviewByIdQuery,
            Variables = new ReviewByIdVariables
            {
                Id = id
            }
        };

        GraphQlResponse<ReviewByIdResponseData>? responseBody;

        try
        {
            using var response = await httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ReviewQueryResult
                {
                    Success = false,
                    ErrorMessage = $"Errore HTTP verso GraphQL: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<ReviewByIdResponseData>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ReviewQueryResult
            {
                Success = false,
                ErrorMessage = $"Impossibile contattare il backend GraphQL: {ex.Message}"
            };
        }

        if (responseBody is null)
        {
            return new ReviewQueryResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL non valida o non deserializzabile."
            };
        }

        if (responseBody.Errors is { Count: > 0 })
        {
            var errorMessage = string.Join(" | ", responseBody.Errors.Select(error => error.Message));
            return new ReviewQueryResult
            {
                Success = false,
                ErrorMessage = $"GraphQL ha restituito errori: {errorMessage}"
            };
        }

        return new ReviewQueryResult
        {
            Success = true,
            Review = responseBody.Data?.ReviewById
        };
    }

    public async Task<ReviewMutationResult> AddReviewAsync(
        int rate,
        string comment,
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        if (rate <= 0)
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = "Rate non valido."
            };
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = "Commento obbligatorio."
            };
        }

        if (employeeId <= 0)
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = "EmployeeId non valido."
            };
        }

        var request = new GraphQlRequest<AddReviewVariables>
        {
            Query = AddReviewMutation,
            Variables = new AddReviewVariables
            {
                Input = new ReviewInputVariables
                {
                    Rate = rate,
                    Comment = comment,
                    EmployeeId = employeeId
                }
            }
        };

        GraphQlResponse<AddReviewResponseData>? responseBody;

        try
        {
            using var response = await httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ReviewMutationResult
                {
                    Success = false,
                    ErrorMessage = $"Errore HTTP verso GraphQL: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<AddReviewResponseData>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = $"Impossibile contattare il backend GraphQL: {ex.Message}"
            };
        }

        if (responseBody is null)
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL non valida o non deserializzabile."
            };
        }

        if (responseBody.Errors is { Count: > 0 })
        {
            var errorMessage = string.Join(" | ", responseBody.Errors.Select(error => error.Message));
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = $"GraphQL ha restituito errori: {errorMessage}"
            };
        }

        var review = responseBody.Data?.AddReview;
        return new ReviewMutationResult
        {
            Success = review is not null,
            ErrorMessage = review is null ? "Add review non eseguita." : null,
            Review = review
        };
    }

    public async Task<ReviewMutationResult> UpdateReviewAsync(
        int id,
        int rate,
        string comment,
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = "Id review non valido."
            };
        }

        if (rate <= 0)
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = "Rate non valido."
            };
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = "Commento obbligatorio."
            };
        }

        if (employeeId <= 0)
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = "EmployeeId non valido."
            };
        }

        var request = new GraphQlRequest<UpdateReviewVariables>
        {
            Query = UpdateReviewMutation,
            Variables = new UpdateReviewVariables
            {
                Id = id,
                Input = new ReviewInputVariables
                {
                    Rate = rate,
                    Comment = comment,
                    EmployeeId = employeeId
                }
            }
        };

        GraphQlResponse<UpdateReviewResponseData>? responseBody;

        try
        {
            using var response = await httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ReviewMutationResult
                {
                    Success = false,
                    ErrorMessage = $"Errore HTTP verso GraphQL: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<UpdateReviewResponseData>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = $"Impossibile contattare il backend GraphQL: {ex.Message}"
            };
        }

        if (responseBody is null)
        {
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = "Risposta GraphQL non valida o non deserializzabile."
            };
        }

        if (responseBody.Errors is { Count: > 0 })
        {
            var errorMessage = string.Join(" | ", responseBody.Errors.Select(error => error.Message));
            return new ReviewMutationResult
            {
                Success = false,
                ErrorMessage = $"GraphQL ha restituito errori: {errorMessage}"
            };
        }

        var review = responseBody.Data?.UpdateReview;
        return new ReviewMutationResult
        {
            Success = review is not null,
            ErrorMessage = review is null ? "Update review non eseguita." : null,
            Review = review
        };
    }

    public async Task<GraphQlOperationResult> DeleteReviewAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return new GraphQlOperationResult
            {
                Success = false,
                ErrorMessage = "Id review non valido."
            };
        }

        var request = new GraphQlRequest<DeleteReviewVariables>
        {
            Query = DeleteReviewMutation,
            Variables = new DeleteReviewVariables
            {
                Id = id
            }
        };

        GraphQlResponse<DeleteReviewResponseData>? responseBody;

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

            responseBody = await response.Content.ReadFromJsonAsync<GraphQlResponse<DeleteReviewResponseData>>(cancellationToken: cancellationToken);
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
            Success = responseBody.Data?.DeleteReview ?? false,
            ErrorMessage = responseBody.Data?.DeleteReview == true
                ? null
                : "Delete review non eseguita: review non trovata."
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
