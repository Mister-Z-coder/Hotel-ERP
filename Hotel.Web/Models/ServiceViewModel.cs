using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ServiceViewModel
    {
        public IEnumerable<Service> Service { get; set; }
        public string? SearchString { get; set; }
    }
}
