using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class Customer
    {
        public Customer()
        {
            Accounts = new HashSet<Account>();
            Orders = new HashSet<Order>();
        }

        public string CustomerId { get; set; }
        [Required(ErrorMessage = "Company Name is Required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Contact Name is Required")]
        public string ContactName { get; set; }
        [Required(ErrorMessage = "Contact Title is Required")]
        public string ContactTitle { get; set; }
        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
