using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class DetailsSortie
    {
        public int DetailsSortieId { get; set; } // Identifiant unique pour les détails de la commande
        public int SortieId { get; set; }
        public string Agent { get; set; }
        public string TypeMouvement { get; set; }
        public DateTime DateMouvement { get; set; }
        public DateTime HeureMouvement { get; set; }
        public string NatureMouvement { get; set; }
        public int AjoutId { get; set; }
        public string MotifAjout { get; set; }
        public string ProduitLivre { get; set; }
        public int QteLivre { get; set; }
    }
}
