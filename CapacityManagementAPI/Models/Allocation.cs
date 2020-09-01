using System;

namespace CapacityManagementAPI.Models
{
    public partial class Allocation
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Allocation1 { get; set; }
        public double? WorkWeight { get; set; }
        public string Role { get; set; }
        public Boolean isUpdate { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Project Project { get; set; }

    }
}
