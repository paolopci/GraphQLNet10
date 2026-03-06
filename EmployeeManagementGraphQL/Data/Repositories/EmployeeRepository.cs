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
        IEnumerable<EmployeeSortCriterion>? sortCriteria)
    {
        var request = PaginationRequest.Normalize(page, pageSize);

        IQueryable<Employee> query = _context.EmployeeEntity.AsNoTracking();

        query = ApplySorting(query, sortCriteria);

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
        IEnumerable<EmployeeSortCriterion>? sortCriteria)
    {
        var normalizedCriteria = (sortCriteria ?? [])
            .Where(c => c is not null)
            .GroupBy(c => c.Field)
            .Select(g => g.First())
            .ToList();

        if (normalizedCriteria.Count == 0)
        {
            return query.OrderBy(e => e.Id);
        }

        IOrderedQueryable<Employee>? orderedQuery = null;

        foreach (var criterion in normalizedCriteria)
        {
            orderedQuery = ApplySingleCriterion(orderedQuery, query, criterion);
        }

        return orderedQuery ?? query.OrderBy(e => e.Id);
    }

    private static IOrderedQueryable<Employee> ApplySingleCriterion(
        IOrderedQueryable<Employee>? orderedQuery,
        IQueryable<Employee> baseQuery,
        EmployeeSortCriterion criterion)
    {
        var isDescending = criterion.Direction == SortDirection.Desc;

        return criterion.Field switch
        {
            EmployeeSortField.FirstName => ApplyOrdering(
                orderedQuery,
                baseQuery,
                e => e.FirstName,
                isDescending),
            EmployeeSortField.LastName => ApplyOrdering(
                orderedQuery,
                baseQuery,
                e => e.LastName,
                isDescending),
            EmployeeSortField.Email => ApplyOrdering(
                orderedQuery,
                baseQuery,
                e => e.Email,
                isDescending),
            _ => ApplyOrdering(
                orderedQuery,
                baseQuery,
                e => e.Id,
                isDescending)
        };
    }

    private static IOrderedQueryable<Employee> ApplyOrdering<TKey>(
        IOrderedQueryable<Employee>? orderedQuery,
        IQueryable<Employee> baseQuery,
        System.Linq.Expressions.Expression<Func<Employee, TKey>> keySelector,
        bool descending)
    {
        if (orderedQuery is null)
        {
            return descending
                ? baseQuery.OrderByDescending(keySelector)
                : baseQuery.OrderBy(keySelector);
        }

        return descending
            ? orderedQuery.ThenByDescending(keySelector)
            : orderedQuery.ThenBy(keySelector);
    }
}


