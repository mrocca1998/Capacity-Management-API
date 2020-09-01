using System;
using System.Collections.Generic;

namespace CapacityManagementAPI.Models
{
    public partial class Project
    {

        public Project()
        {
            Allocations = new List<Allocation>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TotalPoints { get; set; }
        public int? BaPoints { get; set; }
        public int? QaPoints { get; set; }
        public int? DevPoints { get; set; }
        public DateTime? baEndDate { get; set; }
        public DateTime? qaEndDate { get; set; }
        public DateTime? devEndDate { get; set; }
        public DateTime? calcEndDate { get; set; }
        public Boolean? isUpdate { get; set; }

        public virtual ICollection<Allocation> Allocations { get; set; }
    }
}
