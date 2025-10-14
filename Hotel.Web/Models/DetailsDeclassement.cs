using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class DetailsDeclassement
    {
        public int DetailsDeclassementId { get; set; } // Identifiant unique pour les détails de la commande
        public int DeclassementId { get; set; }
        public string Agent { get; set; }
        public string TypeMouvement { get; set; }
        public DateTime DateMouvement { get; set; }
        public DateTime HeureMouvement { get; set; }
        public string NatureMouvement { get; set; }
        public string MotifDeclassement { get; set; }
        public string ProduitDeclasse { get; set; }
        public int QteDeclasse { get; set; }
    }
}
