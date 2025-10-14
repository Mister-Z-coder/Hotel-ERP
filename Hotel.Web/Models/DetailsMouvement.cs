using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models
{
    public class DetailsMouvement
    {
        public int DetailsMouvementId { get; set; }
        public int MouvementId { get; set; }
        public string Agent { get; set; }
        public string TypeMouvement { get; set; }
        public DateTime DateMouvement { get; set; }
        public DateTime HeureMouvement { get; set; }
        public string NatureMouvement { get; set; }
        public string ProduitMouvement { get; set; }
        public int QteMouvement { get; set; }
    }
}
