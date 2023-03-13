using System;
using System.Collections.Generic;

#nullable disable

namespace NokNok_ShoppingAPI.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Accounts = new HashSet<Account>();
            Orders = new HashSet<Order>();
        }

        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
