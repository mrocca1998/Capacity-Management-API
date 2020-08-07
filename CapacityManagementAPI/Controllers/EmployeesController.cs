using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CapacityManagementAPI.Models;

namespace CapacityManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly capManContext _context;

        public EmployeesController(capManContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.OrderBy(e => e.name).ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }
        //api/employees/details/id
        [HttpGet("Details/{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeDetails(int id)
        {
            var employee = _context.Employees.Include(e => e.Allocations).FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        public async Task<string> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return "";
            }

            if (_context.Employees.Any(e => (e.name == employee.name && e.Id != id)))
            {
                return "There already exists an employee with that name. Enter a unique name.";
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return "";
                }
                else
                {
                    throw;
                }
            }

            return "";
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<ActionResult<string>> PostEmployee(Employee employee)
        {
            if (_context.Employees.Any(e => e.name == employee.name))
            {
                return "There already exists an employee with that name. Enter a unique name.";
            }

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return "";
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        
            return employee;
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
