using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models
{
    public class Declassement:Mouvement
    {

        [Required(ErrorMessage = "Motif decaissement requis")]
        [Display(Name = "Motif decaissement")]
        public string MotifDecaissement { get; set; }


    }
}
