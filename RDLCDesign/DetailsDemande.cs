namespace RDLCDesign
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DetailsDemande
    {
        public int DetailsDemandeId { get; set; }

        public int DemandeId { get; set; }

        public string Agent { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateDemande { get; set; }

        public string TypeDemande { get; set; }

        public string EtatDemande { get; set; }

        public string ProduitDemande { get; set; }

        public int QteDemande { get; set; }
    }
}
