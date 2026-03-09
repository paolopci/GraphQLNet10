using EmployeeManagementGraphQL.Data.Models;
using EmployeeManagementGraphQL.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementGraphQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(EmployeeRepository employeeRepository) : ControllerBase
    {
        private readonly EmployeeRepository _employeeRepository = employeeRepository;

        [HttpGet]
        public IActionResult GetAll()
        {
            var allEmployees = _employeeRepository.GetAllEmployees();
            return Ok(allEmployees);
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployeeById(int id)
        {
            var employee = _employeeRepository.GetEmployeeById(id);
            if (employee != null)
            {
                return Ok(employee);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            var addedEmployee = _employeeRepository.AddEmployee(employee);
            return Ok(addedEmployee);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, Employee employee)
        {
            var updateEmployee = _employeeRepository.UpdateEmployee(id, employee);
            if (updateEmployee != null)
            {
                return Ok(updateEmployee);
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            _employeeRepository.DeleteEmployee(id);
            return NoContent();
        }
    }
}
