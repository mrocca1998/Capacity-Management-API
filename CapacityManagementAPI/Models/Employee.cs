using System;
using System.Collections.Generic;

namespace CapacityManagementAPI.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Allocations = new HashSet<Allocation>();
        }

        public int Id { get; set; }
        public string name { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Allocation> Allocations { get; set; }
    }
}
