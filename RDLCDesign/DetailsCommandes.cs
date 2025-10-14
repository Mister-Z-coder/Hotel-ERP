namespace RDLCDesign
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DetailsCommandes
    {
        [Key]
        public int DetailsCommandeId { get; set; }

        public int CommandeId { get; set; }

        public string ProduitNom { get; set; }

        public int Quantite { get; set; }

        public float PrixUnitaire { get; set; }

        public string TypeCommande { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCommande { get; set; }

        public string NomsAgent { get; set; }

        public string NomsClient { get; set; }

        public string TitreTable { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime HeureCommande { get; set; }

        public string TypeProduit { get; set; }
    }
}
