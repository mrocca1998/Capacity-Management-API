using CapacityManagementAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CapacityManagementAPI.Models
{
    public class ProjectData
    {
        private ProjectRepo projectRepo;
        public ProjectData(Project project)
        {
            this.projectRepo = new ProjectRepo();
            taskId = project.Id.ToString();
            taskName = project.Title;
            resource = null;
            startDate = (DateTime)project.StartDate;
            DateTime baEndDate = this.projectRepo.GetEndDate((DateTime)project.StartDate, "BA", Convert.ToDouble(project.BaPoints), project.Allocations);
            DateTime qaEndDate = this.projectRepo.GetEndDate((DateTime)project.StartDate, "QA", Convert.ToDouble(project.QaPoints), project.Allocations);
            DateTime devEndDate = this.projectRepo.GetEndDate((DateTime)project.StartDate, "Dev", Convert.ToDouble(project.DevPoints), project.Allocations);
            endDate = new[] { baEndDate, qaEndDate, devEndDate }.Max();
            duration = null;
            percentComplete = (int)((project.BaPoints + project.QaPoints + project.DevPoints)/3 / project.TotalPoints * 100);
            dependencies = null;

        }

        public void updateDate(Project project)
        {
            DateTime baEndDate = this.projectRepo.GetEndDate((DateTime)project.StartDate, "BA", Convert.ToDouble(project.BaPoints), project.Allocations);
            DateTime qaEndDate = this.projectRepo.GetEndDate((DateTime)project.StartDate, "QA", Convert.ToDouble(project.QaPoints), project.Allocations);
            DateTime devEndDate = this.projectRepo.GetEndDate((DateTime)project.StartDate, "Dev", Convert.ToDouble(project.DevPoints), project.Allocations);
            this.endDate = new[] { baEndDate, qaEndDate, devEndDate }.Max();
        }
        public string taskId { get; set; }
        public string taskName { get; set; }
        public string resource { get; set; } 
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int? duration { get; set; }
        public int percentComplete { get; set; }
        public string dependencies { get; set; }


    }
}
