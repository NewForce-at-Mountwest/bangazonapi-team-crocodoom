﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string AcctNumber { get; set; }
        public string Name { get; set; }
        public int CustomerId { get; set; }
    }
}

