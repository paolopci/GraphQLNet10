using EmployeeManagementGraphQL.Data.Models;
using EmployeeManagementGraphQL.Data.Models.Paging;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementGraphQL.Data.Repositories;

public class EmployeeRepository
{
    private readonly EntityDatabaseContext _context;

    public EmployeeRepository(EntityDatabaseContext context)
    {
        _context = context;
    }

    public List<Employee> GetAllEmployees()
    {
        return [.. _context.EmployeeEntity.Include(f => f.Reviews)];
    }

    public PagedResult<Employee> GetEmployeesPaged(
        int page,
        int pageSize,
        EmployeeSortField sortBy,
        SortDirection sortDirection)
    {
        var request = PaginationRequest.Normalize(page, pageSize);

        IQueryable<Employee> query = _context.EmployeeEntity.AsNoTracking();

        query = ApplySorting(query, sortBy, sortDirection);

        var totalCount = query.Count();
        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var pageInfo = PageInfo.From(totalCount, request.Page, request.PageSize);

        return new PagedResult<Employee>(items, pageInfo);
    }

    public Employee? GetEmployeeById(int id)
    {
        return _context.EmployeeEntity.Include(t => t.Reviews).Where(d => d.Id == id).FirstOrDefault();
    }

    public Employee AddEmployee(Employee employee)
    {
        _context.EmployeeEntity.Add(employee);
        _context.SaveChanges();
        return employee;
    }

    public Employee? UpdateEmployee(int id, Employee employee)
    {
        var _employee = _context.EmployeeEntity.Where(d => d.Id == id).FirstOrDefault();
        if (_employee != null)
        {
            _employee.FirstName = employee.FirstName;
            _employee.LastName = employee.LastName;
            _employee.Email = employee.Email;
        }
        _context.SaveChanges();
        return _employee;
    }

    public void DeleteEmployee(int id)
    {
        var _employee = _context.EmployeeEntity.Find(id);
        if (_employee != null)
        {
            _context.EmployeeEntity.Remove(_employee);
            _context.SaveChanges();
        }
    }

    private static IQueryable<Employee> ApplySorting(
        IQueryable<Employee> query,
        EmployeeSortField sortBy,
        SortDirection sortDirection)
    {
        return sortBy switch
        {
            EmployeeSortField.FirstName => sortDirection == SortDirection.Desc
                ? query.OrderByDescending(e => e.FirstName)
                : query.OrderBy(e => e.FirstName),
            EmployeeSortField.LastName => sortDirection == SortDirection.Desc
                ? query.OrderByDescending(e => e.LastName)
                : query.OrderBy(e => e.LastName),
            EmployeeSortField.Email => sortDirection == SortDirection.Desc
                ? query.OrderByDescending(e => e.Email)
                : query.OrderBy(e => e.Email),
            _ => sortDirection == SortDirection.Desc
                ? query.OrderByDescending(e => e.Id)
                : query.OrderBy(e => e.Id)
        };
    }
}


