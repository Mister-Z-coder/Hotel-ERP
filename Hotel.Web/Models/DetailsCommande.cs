using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class DetailsCommande
    {
        public int DetailsCommandeId { get; set; } // Identifiant unique pour les détails de la commande
        public int CommandeId { get; set; } // Identifiant de la commande associée
        public string ProduitNom { get; set; } // Nom du produit
        public int Quantite { get; set; } // Quantité commandée
        public float PrixUnitaire { get; set; } // Prix par unité
        public float MontantTotal => Quantite * PrixUnitaire; // Montant total pour cet article
        public string TypeCommande { get; set; } // Type de produit
        public string NomsClient { get; set; } // Nom client
        public string NomsAgent { get; set; } // Nom client
        public string TitreTable { get; set; } // Nom client
        public DateTime DateCommande { get; set; } // Date commande de produit
        public DateTime HeureCommande { get; set; } // Date commande de produit
        public string TypeProduit { get; set; } // Date commande de produit
    }
}
