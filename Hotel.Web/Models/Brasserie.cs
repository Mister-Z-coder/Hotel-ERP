using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Brasserie
    {
        public int BrasserieId{ get; set; }

        [Required(ErrorMessage = "Titre categorie requis")]
        [Display(Name = "Titre categorie")]
        public string TitreBrasserie { get; set; }
    }
}
