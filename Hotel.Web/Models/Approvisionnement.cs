using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Approvisionnement : Demande
    {
        public string EtatDemandeAppro { get; set; }
    }
}
