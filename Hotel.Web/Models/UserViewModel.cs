using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class UserViewModel
    {
        public IEnumerable<User> User { get; set; }
        public string? SearchString { get; set; }
    }
}
