using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class AgentViewModel
    {
        public IEnumerable<Agent> Agent { get; set; }
        public string? SearchString { get; set; }
    }
}
