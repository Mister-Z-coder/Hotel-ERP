using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class PosteTravailsViewModel
    {
        public IEnumerable<PosteTravail> PosteTravail { get; set; }
        public string? SearchString { get; set; }
    }
}
