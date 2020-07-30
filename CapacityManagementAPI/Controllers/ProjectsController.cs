using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CapacityManagementAPI.Models;
using Newtonsoft.Json;
using CapacityManagementAPI.Services;
using Project = CapacityManagementAPI.Models.Project;

namespace CapacityManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly capManContext _context;

        private ProjectRepo projectRepo;

        public ProjectsController(capManContext context)
        {
            _context = context;
            this.projectRepo = new ProjectRepo();
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var projects = await _context.Projects.Include(p => p.Allocations).ToListAsync();
            for (int runs = 0; runs < projects.Count; runs++)
            {
                var project = projects[runs];
                project.baEndDate = this.projectRepo.GetEndDate((DateTime)project.StartDate, "BA", Convert.ToDouble(project.BaPoints), project.Allocations);
                project.qaEndDate = this.projectRepo.GetEndDate((DateTime)project.StartDate, "QA", Convert.ToDouble(project.QaPoints), project.Allocations);
                project.devEndDate = this.projectRepo.GetEndDate((DateTime)project.StartDate, "Dev", Convert.ToDouble(project.DevPoints), project.Allocations);
                project.calcEndDate = new[] { project.baEndDate, project.qaEndDate, project.devEndDate }.Max();

            }
            return projects;
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        //api/projects/details/id
        [HttpGet("Details/{id}")]
        public async Task<ActionResult<Project>> GetProjectDetails(int id)
        {
            var project = _context.Projects.Include(p => p.Allocations).ThenInclude(al => al.Employee).Where(p => p.Id == id).FirstOrDefault();

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        //api/projects/duration/id
        [HttpGet("Duration/{id}")]
        public async Task<ActionResult<string>> GetProjectDuration(int id)
        {
            var project = _context.Projects.Include(p => p.Allocations).FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var baEndDate = projectRepo.GetEndDate((DateTime)project.StartDate, "BA", Convert.ToDouble(project.BaPoints), project.Allocations);
            var qaEndDate = projectRepo.GetEndDate((DateTime)project.StartDate, "QA", Convert.ToDouble(project.QaPoints), project.Allocations);
            var devEndDate = projectRepo.GetEndDate((DateTime)project.StartDate, "Dev", Convert.ToDouble(project.DevPoints), project.Allocations);
            var projectEndDate = new[] { baEndDate, qaEndDate, devEndDate }.Max();

            var points = new Dictionary<string, DateTime>
            {
                { "baEndDate", baEndDate },
                { "qaEndDate", qaEndDate },
                { "devEndDate", devEndDate },
                { "projectEndDate", projectEndDate }
            };
            var dates = JsonConvert.SerializeObject(points, Formatting.Indented);
            return dates;
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id,[FromBody]Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (project.EndDate.HasValue && DateTime.Compare((DateTime)project.StartDate, (DateTime)project.EndDate) > 0)
            {
                throw new ArgumentException("Entry error: Project start date is after the end date", "original");
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }



        // POST: api/Projects
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            if (project.EndDate.HasValue && DateTime.Compare((DateTime)project.StartDate, (DateTime)project.EndDate) > 0)
            {
                throw new ArgumentException("Entry error: Project start date is after the end date", "original");
            }
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw (e);
            }
            return project;
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
