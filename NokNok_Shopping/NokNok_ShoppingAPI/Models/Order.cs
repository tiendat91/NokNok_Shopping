﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace NokNok_ShoppingAPI.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
        [Key]
        public int OrderId { get; set; }
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
