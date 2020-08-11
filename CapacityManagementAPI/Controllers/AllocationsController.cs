using System;
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
    public class AllocationsController : ControllerBase
    {
        private readonly capManContext _context;

        public AllocationsController(capManContext context)
        {
            _context = context;
        }

        // GET: api/Allocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Allocation>>> GetAllocations()
        {
            return await _context.Allocations.OrderBy(a => a.StartDate).ToListAsync();
        }

        // GET: api/Allocations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Allocation>> GetAllocation(int id)
        {
            var allocation = await _context.Allocations.FindAsync(id);

            if (allocation == null)
            {
                return NotFound();
            }

            return allocation;
        }
        //Allocations/Details/id
        [HttpGet("Details/{id}")]
        public async Task<ActionResult<Allocation>> GetAllocationDetails(int id)
        {
            var allocation =  _context.Allocations.Include(al => al.Employee).Include(al => al.Project).FirstOrDefault(al => al.Id == id);

            if (allocation == null)
            {
                return NotFound();
            }
             
            return allocation;
        }

        // PUT: api/Allocations/Check/5
        [HttpPut("Check/{id}")]
        public async Task<ActionResult<String>> PutAllocationCheck(int id, [FromBody] Allocation allocation)
        {
            Dictionary<int, string> months = new Dictionary<int, string> { { 1, "January" }, { 2, "February" }, { 3, "March" }, { 4, "April" }, { 5, "May" }, { 6, "June" }, { 7, "July" }, { 8, "August" }, { 9, "September" }, { 10, "October" }, { 11, "November" }, { 12, "December" } };
            if (!allocation.EndDate.HasValue)
            {
                allocation.EndDate = allocation.StartDate;
            }
            if (DateTime.Compare((DateTime)allocation.StartDate, (DateTime)allocation.EndDate) > 0)
            {
                string message = "Entry error: Allocation start date is after the end date";
                return message;
            }

            var currentAllo2 = 0;
            var doubleAllo = new Dictionary<DateTime, int>();
            DateTime date = (DateTime)allocation.StartDate;
            var allocations = await _context.Allocations.ToListAsync();


            //check if the current month is within the allocation's span
            while (DateTime.Compare(date, (DateTime)allocation.EndDate) <= 0)
            {
                currentAllo2 = 0;
                foreach (Allocation allocation2 in allocations)
                {
                    //check if an allocation contains the current month and is assigned to the same employee
                    if (DateTime.Compare(date, (DateTime)allocation2.StartDate) >= 0
                            && DateTime.Compare(date, (DateTime)allocation2.EndDate) <= 0
                            && allocation.EmployeeId.Equals(allocation2.EmployeeId)
                            && allocation.ProjectId.Equals(allocation2.ProjectId)
                            && allocation.Role.Equals(allocation2.Role)
                            && id != allocation2.Id)
                    {
                        currentAllo2 = allocation2.Id;
                    }
                }
                if (currentAllo2 > 0)
                {
                    doubleAllo[date] = currentAllo2;
                }

                date = date.AddMonths(1);
            }

            //check for double allocation
            if (doubleAllo.Count > 0)
            {
                string dates = "";
                foreach (DateTime date1 in doubleAllo.Keys)
                {
                    dates += Environment.NewLine + months[date1.Month] + " " + date1.Year.ToString();
                }
                string message = "Error: " + _context.Employees.FirstOrDefault(e => e.Id == allocation.EmployeeId).name + " is already allocated as a " + allocation.Role + " to " + _context.Projects.FirstOrDefault(p => p.Id == allocation.ProjectId).Title + dates;
                return message;

            }

            date = (DateTime)allocation.StartDate;
            allocations = await _context.Allocations.ToListAsync();
            var overAllo = new Dictionary<DateTime, List<int>>();
            var currentAllo = new List<int>();
            var alloTotal = allocation.Allocation1;

            //check if the current month is within the allocation's span
            while (DateTime.Compare(date, (DateTime)allocation.EndDate) <= 0)
            {
                currentAllo = new List<int>();
                //running count of the total allocation for that employee that month
                alloTotal = allocation.Allocation1;
                foreach (Allocation allocation2 in allocations)
                {
                    //check if an allocation contains the current month and is assigned to the same employee
                    if (DateTime.Compare(date, (DateTime)allocation2.StartDate) >= 0
                            && DateTime.Compare(date, (DateTime)allocation2.EndDate) <= 0
                            && allocation.EmployeeId.Equals(allocation2.EmployeeId)
                            && id != allocation2.Id)
                    {
                        //add that allocation to the current toal of that employee's allocations for the month and save the allocation id
                        currentAllo.Add(allocation2.Id);
                        alloTotal += allocation2.Allocation1;
                    }
                    //check if the employee is overallocated at the current month
                    if (alloTotal > 100)
                    {
                        //save the month and the list of allocation id's for that month
                        overAllo[date] = currentAllo;
                    }
                }
                date = date.AddMonths(1);
            }
            string overAlloDates = "";
            double totalAllo = 0;
            if (overAllo.Count > 0)
            {
                foreach (KeyValuePair<DateTime, List<int>> date2 in overAllo)
                {
                    totalAllo = (double)allocation.Allocation1;
                    overAlloDates += Environment.NewLine + Environment.NewLine + months[date2.Key.Month] + " " + date2.Key.Year.ToString();
                    foreach (int allocationID in date2.Value)
                    {
                        Allocation allocation2 = _context.Allocations.FirstOrDefault(a => a.Id == allocationID);
                        totalAllo += (double)allocation2.Allocation1;
                        overAlloDates += Environment.NewLine + _context.Projects.FirstOrDefault(p => p.Id == allocation2.ProjectId).Title + " " + allocation2.Allocation1 + "%";
                    }
                    overAlloDates += Environment.NewLine + _context.Projects.FirstOrDefault(p => p.Id == allocation.ProjectId).Title + " " + allocation.Allocation1 + "%";
                    overAlloDates += Environment.NewLine + "Total Allocation: " + totalAllo + "%";
                }
                string eMessage = "Entry error: " + _context.Employees.FirstOrDefault(e => e.Id == allocation.EmployeeId).name + " would be overallocated:" + overAlloDates;
                return eMessage;

            }

            return "";
        }

            // PUT: api/Allocations/5
            [HttpPut("{id}")]
        public async Task<ActionResult<String>> PutAllocation(int id, [FromBody]Allocation allocation)
        {

            Dictionary<int, string> months = new Dictionary<int, string> { { 1, "January" }, { 2, "February" }, { 3, "March" }, { 4, "April" }, { 5, "May" }, { 6, "June" }, { 7, "July" }, { 8, "August" }, { 9, "September" }, { 10, "October" }, { 11, "November" }, { 12, "December" } };

            if (id != allocation.Id)
            {
                return BadRequest();
            }

            if (!allocation.EndDate.HasValue)
            {
                allocation.EndDate = allocation.StartDate;
            }


            if (DateTime.Compare((DateTime)allocation.StartDate, (DateTime)allocation.EndDate) > 0)
            {
                return "Entry error: Project start date is after the end date";
            }

            _context.Entry(allocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllocationExists(id))
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

        // POST: api/Allocations
        [HttpPost]
        public async Task<ActionResult<String>> PostAllocation(Allocation allocation)
        {
            Dictionary<int, string> months = new Dictionary<int, string> { {1, "January"}, { 2, "February" }, { 3, "March" }, { 4, "April" }, { 5, "May" }, { 6, "June" }, {7, "July" }, { 8, "August" }, { 9, "September" }, { 10, "October" }, { 11, "November" }, { 12, "December" } };
            if (!allocation.EndDate.HasValue)
            {
                allocation.EndDate = allocation.StartDate;
            }
            if (DateTime.Compare((DateTime)allocation.StartDate, (DateTime)allocation.EndDate) > 0)
            {
                string message = "Entry error: Allocation start date is after the end date";
                return message;
            }

            var currentAllo2 = 0;
            var doubleAllo = new Dictionary<DateTime, int>();
            DateTime date = (DateTime)allocation.StartDate;
            var allocations = await _context.Allocations.ToListAsync();


            //check if the current month is within the allocation's span
            while (DateTime.Compare(date, (DateTime)allocation.EndDate) <= 0)
            {
                currentAllo2 = 0;
                foreach (Allocation allocation2 in allocations)
                {
                    //check if an allocation contains the current month and is assigned to the same employee
                    if (DateTime.Compare(date, (DateTime)allocation2.StartDate) >= 0
                            && DateTime.Compare(date, (DateTime)allocation2.EndDate) <= 0
                            && allocation.EmployeeId.Equals(allocation2.EmployeeId)
                            && allocation.ProjectId.Equals(allocation2.ProjectId)
                            && allocation.Role.Equals(allocation2.Role))
                    {
                        currentAllo2 = allocation2.Id;
                    }
                }
                if (currentAllo2 > 0)
                {
                    doubleAllo[date] = currentAllo2;
                }

                date = date.AddMonths(1);
            }

            //check for double allocation
            if (doubleAllo.Count > 0)
            {
                string dates = "";
                foreach (DateTime date1 in doubleAllo.Keys)
                {
                    dates += Environment.NewLine + months[date1.Month] + " " + date1.Year.ToString();
                }
                string message = "Error: " + _context.Employees.FirstOrDefault(e => e.Id == allocation.EmployeeId).name + " is already allocated as a " + allocation.Role + " to " + _context.Projects.FirstOrDefault(p => p.Id == allocation.ProjectId).Title + dates;
                return message;

            }

            date = (DateTime)allocation.StartDate;
            allocations = await _context.Allocations.ToListAsync();
            var overAllo = new Dictionary<DateTime, List<int>>();
            var currentAllo = new List<int>();
            var alloTotal = allocation.Allocation1;

            //check if the current month is within the allocation's span
            while (DateTime.Compare(date, (DateTime)allocation.EndDate) <= 0)
            {
                currentAllo = new List<int>();
                //running count of the total allocation for that employee that month
                alloTotal = allocation.Allocation1;
                foreach (Allocation allocation2 in allocations)
                {
                    //check if an allocation contains the current month and is assigned to the same employee
                    if (DateTime.Compare(date, (DateTime)allocation2.StartDate) >= 0
                            && DateTime.Compare(date, (DateTime)allocation2.EndDate) <= 0
                            && allocation.EmployeeId.Equals(allocation2.EmployeeId))
                    {
                        //add that allocation to the current toal of that employee's allocations for the month and save the allocation id
                        currentAllo.Add(allocation2.Id);
                        alloTotal += allocation2.Allocation1;
                    }
                    //check if the employee is overallocated at the current month
                    if (alloTotal > 100)
                    {
                        //save the month and the list of allocation id's for that month
                        overAllo[date] = currentAllo;
                    }
                }
                date = date.AddMonths(1);
            }
            string overAlloDates = "";
            double totalAllo = 0;
            if (overAllo.Count > 0)
            {
                foreach (KeyValuePair<DateTime, List<int>> date2 in overAllo)
                {
                    totalAllo = (double)allocation.Allocation1;
                    overAlloDates += Environment.NewLine + Environment.NewLine + months[date2.Key.Month] + " " + date2.Key.Year.ToString();
                    foreach(int allocationID in date2.Value)
                    {
                        Allocation allocation2 = _context.Allocations.FirstOrDefault(a => a.Id == allocationID);
                        totalAllo += (double)allocation2.Allocation1;
                        overAlloDates += Environment.NewLine + _context.Projects.FirstOrDefault(p => p.Id == allocation2.ProjectId).Title + " " + allocation2.Allocation1 + "%";
                    }
                    overAlloDates += Environment.NewLine + _context.Projects.FirstOrDefault(p => p.Id == allocation.ProjectId).Title + " " + allocation.Allocation1 + "%";
                    overAlloDates += Environment.NewLine + "Total Allocation: " + totalAllo + "%";
                }
                string eMessage = "Entry error: " + _context.Employees.FirstOrDefault(e => e.Id == allocation.EmployeeId).name + " would be overallocated:" + overAlloDates;
                return eMessage;

            }

            

            _context.Allocations.Add(allocation);
            await _context.SaveChangesAsync();

            return "";
        }

        // DELETE: api/Allocations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Allocation>> DeleteAllocation(int id)
        {
            var allocation = await _context.Allocations.FindAsync(id);
            if (allocation == null)
            {
                return NotFound();
            }

            _context.Allocations.Remove(allocation);
            await _context.SaveChangesAsync();

            return allocation;
        }

        private bool AllocationExists(int id)
        {
            return _context.Allocations.Any(e => e.Id == id);
        }
    }
}
