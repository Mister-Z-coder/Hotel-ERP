using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class DetailsReglement
    {
        public int DetailsReglementId { get; set; }
        public int ReglementId { get; set; }
        public string ModeReglement { get; set; }
        public string Devise { get; set; }
        public string TypeReglement { get; set; }
        public string  Agent {get;set;}
        public string  Caisse { get; set; }
        public DateTime DateReglement { get; set; }
        public DateTime HeureReglement { get; set; }
        public decimal MontantReglement { get; set; }
        public string MotifReglement { get; set; }

    }
}
