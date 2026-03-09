using EmployeeManagementGraphQL.Data.Models;
using EmployeeManagementGraphQL.Data.Repositories;
using EmployeeManagementGraphQL.GraphQL.Types;
using GraphQL;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Mutations;

public class EmployeeMutation : ObjectGraphType
{
    public EmployeeMutation(
        EmployeeRepository employeeRepository,
        ReviewRepository reviewRepository)
    {
        Field<EmployeeGraphType>(
            "addEmployee",
            "Add a new employee",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<EmployeeInputType>>
                {
                    Name = "input",
                    Description = "Employee data to create."
                }),
            resolve: context =>
            {
                var input = context.GetArgument<Employee>("input");
                if (input is null)
                {
                    throw new ExecutionError("The 'input' argument is required.");
                }

                var employee = new Employee
                {
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    Email = input.Email
                };

                return employeeRepository.AddEmployee(employee);
            });

        Field<EmployeeGraphType>(
            "updateEmployee",
            "Update an existing employee",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "id",
                    Description = "Employee id to update."
                },
                new QueryArgument<NonNullGraphType<EmployeeInputType>>
                {
                    Name = "input",
                    Description = "Employee data to update."
                }),
            resolve: context =>
            {
                var id = context.GetArgument<int>("id");
                var input = context.GetArgument<Employee>("input");
                if (input is null)
                {
                    throw new ExecutionError("The 'input' argument is required.");
                }

                var employee = new Employee
                {
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    Email = input.Email
                };

                return employeeRepository.UpdateEmployee(id, employee);
            });

        Field<NonNullGraphType<BooleanGraphType>>(
            "deleteEmployee",
            "Delete an employee by id",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "id",
                    Description = "Employee id to delete."
                }),
            resolve: context =>
            {
                var id = context.GetArgument<int>("id");
                var employee = employeeRepository.GetEmployeeById(id);
                if (employee is null)
                {
                    return false;
                }

                employeeRepository.DeleteEmployee(id);
                return true;
            });

        Field<ReviewGraphType>(
            "addReview",
            "Add a new review",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<ReviewInputType>>
                {
                    Name = "input",
                    Description = "Review data to create."
                }),
            resolve: context =>
            {
                var input = context.GetArgument<Review>("input");
                if (input is null)
                {
                    throw new ExecutionError("The 'input' argument is required.");
                }

                if (!reviewRepository.EmployeeExists(input.EmployeeId))
                {
                    throw new ExecutionError($"Employee with id {input.EmployeeId} does not exist.");
                }

                var review = new Review
                {
                    Rate = input.Rate,
                    Comment = input.Comment,
                    EmployeeId = input.EmployeeId
                };

                return reviewRepository.AddReview(review);
            });

        Field<ReviewGraphType>(
            "updateReview",
            "Update an existing review",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "id",
                    Description = "Review id to update."
                },
                new QueryArgument<NonNullGraphType<ReviewInputType>>
                {
                    Name = "input",
                    Description = "Review data to update."
                }),
            resolve: context =>
            {
                var id = context.GetArgument<int>("id");
                var input = context.GetArgument<Review>("input");

                if (input is null)
                {
                    throw new ExecutionError("The 'input' argument is required.");
                }

                if (!reviewRepository.EmployeeExists(input.EmployeeId))
                {
                    throw new ExecutionError($"Employee with id {input.EmployeeId} does not exist.");
                }

                var review = new Review
                {
                    Rate = input.Rate,
                    Comment = input.Comment,
                    EmployeeId = input.EmployeeId
                };

                return reviewRepository.UpdateReview(id, review);
            });

        Field<NonNullGraphType<BooleanGraphType>>(
            "deleteReview",
            "Delete a review by id",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "id",
                    Description = "Review id to delete."
                }),
            resolve: context =>
            {
                var id = context.GetArgument<int>("id");
                return reviewRepository.DeleteReview(id);
            });
    }
}
