namespace RDLCDesign
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DetailsReglement
    {
        public int DetailsReglementId { get; set; }

        public string ModeReglement { get; set; }

        public string Devise { get; set; }

        public string TypeReglement { get; set; }

        public string Agent { get; set; }

        public string Caisse { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateReglement { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime HeureReglement { get; set; }

        public decimal MontantReglement { get; set; }

        public string MotifReglement { get; set; }

        public int ReglementId { get; set; }
    }
}
