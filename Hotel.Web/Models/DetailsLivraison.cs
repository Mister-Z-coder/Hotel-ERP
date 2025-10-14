using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class DetailsLivraison
    {
        public int DetailsLivraisonId { get; set; } // Identifiant unique pour les détails de la commande
        public int LivraisonId { get; set; }
        public string Agent { get; set; }
        public string TypeMouvement { get; set; }
        public DateTime DateMouvement { get; set; }
        public DateTime HeureMouvement { get; set; }
        public string NatureMouvement { get; set; }
        public int ApprovisionnementId { get; set; }
        public string Fournisseur { get; set; }
        public decimal MontantLivraison { get; set; }
        public DateTime DateLivraison { get; set; }
        public DateTime HeureLivraison { get; set; }
        public string ProduitLivre { get; set; }
        public int QteLivre { get; set; }
    }
}
