using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Domain.Entity
{
    class Client
    {
        public int ClientId { get; set; }
        public string NameClient { get; set; }
        public char SexeClient { get; set; }
        public string TypeClient { get; set; }
    }
}
