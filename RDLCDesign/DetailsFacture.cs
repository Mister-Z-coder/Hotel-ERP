namespace RDLCDesign
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DetailsFacture
    {
        public int DetailsFactureId { get; set; }

        public int Numcmd { get; set; }

        public int Numfact { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Datefact { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Heurefact { get; set; }

        public int Caissier { get; set; }

        public int Caisse { get; set; }

        public string Designation { get; set; }

        public int Qte { get; set; }

        public decimal PU { get; set; }

        public string ModeReglement { get; set; }

        public string TypeReglement { get; set; }

        public string Devise { get; set; }

        public decimal Montanttotal { get; set; }
    }
}
