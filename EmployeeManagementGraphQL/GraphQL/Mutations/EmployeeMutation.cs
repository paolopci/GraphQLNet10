using EmployeeManagementGraphQL.Data.Models;
using EmployeeManagementGraphQL.Data.Repositories;
using EmployeeManagementGraphQL.GraphQL.Types;
using GraphQL;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Mutations;

public class EmployeeMutation : ObjectGraphType
{
    public EmployeeMutation(EmployeeRepository employeeRepository)
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
    }
}
