using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ClientViewModel
    {
       
        public Client Client { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public Resident Resident { get; set; }
        public Normal Normal { get; set; }
        public string? SearchString { get; set; }


    }
}
