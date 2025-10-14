using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class HPrixUnitSBBoisson
    {
        public HPrixUnitSBBoisson()
        {
            DateHisto = DateTime.Now;
        }

        public DateTime DateHisto { get; set; }

        [ForeignKey("BoissonId")]
        public int BoissonId { get; set; }
        public Boisson Boisson { get; set; }

        public float PrixUnitSB { get; set; }

    }
}
