﻿using System;
using System.Collections.Generic;

#nullable disable

namespace NokNok_ShoppingAPI.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Accounts = new HashSet<Account>();
            Orders = new HashSet<Order>();
        }

        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int? DepartmentId { get; set; }
        public string? Title { get; set; }
        public string? TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Address { get; set; }

        public virtual Department? Department { get; set; }
        public virtual ICollection<Account>? Accounts { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
