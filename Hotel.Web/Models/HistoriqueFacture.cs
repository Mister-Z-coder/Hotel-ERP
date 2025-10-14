using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class HistoriqueFacture
    {
        public string Numfact { get; set; }
        public string Numcmd { get; set; }
        public string DateRegl { get; set; }
        public string Devise { get; set; }
        public string Client { get; set; }
        public string Agent { get; set; }
        public decimal Montant { get; set; }
    }
}
