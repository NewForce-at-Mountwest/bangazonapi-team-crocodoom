﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime AccountCreated { get; set; }
        public DateTime LastActive { get; set; }

        public List<Product> products { get; set; } = new List<Product>();
        public List<Payment> payments { get; set; } = new List<Payment>();

    }
}
