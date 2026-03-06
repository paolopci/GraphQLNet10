using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementGraphQL.Mvc.Controllers;

public class EmployeesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
