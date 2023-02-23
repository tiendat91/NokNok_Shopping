using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class Account
    {
        public int AccountId { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public int? Role { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
