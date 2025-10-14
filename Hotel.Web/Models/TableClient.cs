using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class TableClient
    {
        public int TableClientId { get; set; }

        [Required(ErrorMessage = "Titre table client requis")]
        [Display(Name = "Titre table client")]
        public string TitreTableClient { get; set; }
    }
}
