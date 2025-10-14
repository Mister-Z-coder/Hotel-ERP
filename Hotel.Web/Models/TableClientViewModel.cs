using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class TableClientViewModel
    {
        public IEnumerable<TableClient> TableClient { get; set; }
        public string? SearchString { get; set; }
    }
}
