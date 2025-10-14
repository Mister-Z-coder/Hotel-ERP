using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class HPrixUnitAliment
    {
        public HPrixUnitAliment()
        {
            DateHisto = DateTime.Now;
        }

        public DateTime DateHisto { get; set; }

        [ForeignKey("AlimentId")]
        public int AlimentId { get; set; }
        public Aliment Aliment { get; set; }
        public float PrixUnitAliment { get; set; }

    }
}
