namespace RDLCDesign
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DetailsFactureReservation
    {
        public int DetailsFactureReservationId { get; set; }

        public int Numres { get; set; }

        public int Numfact { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Datefact { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Heurefact { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Debut { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Fin { get; set; }

        public int Caissier { get; set; }

        public int Caisse { get; set; }

        public string Chambre { get; set; }

        public int Duree { get; set; }

        public decimal Montant { get; set; }

        public string ModeReglement { get; set; }

        public string TypeReglement { get; set; }

        public string Devise { get; set; }

        public string Service { get; set; }
    }
}
