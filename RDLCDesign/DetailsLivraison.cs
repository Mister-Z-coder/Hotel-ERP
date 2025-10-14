namespace RDLCDesign
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DetailsLivraison
    {
        public int DetailsLivraisonId { get; set; }

        public int LivraisonId { get; set; }

        public string Agent { get; set; }

        public string TypeMouvement { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateMouvement { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime HeureMouvement { get; set; }

        public string NatureMouvement { get; set; }

        public int ApprovisionnementId { get; set; }

        public string Fournisseur { get; set; }

        public decimal MontantLivraison { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateLivraison { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime HeureLivraison { get; set; }

        public string ProduitLivre { get; set; }

        public int QteLivre { get; set; }
    }
}
