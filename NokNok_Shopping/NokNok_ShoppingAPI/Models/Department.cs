using System;
using System.Collections.Generic;

#nullable disable

namespace NokNok_ShoppingAPI.Models
{
    public partial class Department
    {
        public Department()
        {
            Employees = new HashSet<Employee>();
        }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentType { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
