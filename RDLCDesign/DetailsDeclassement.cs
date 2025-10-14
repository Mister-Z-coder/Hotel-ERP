namespace RDLCDesign
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DetailsDeclassement
    {
        public int DetailsDeclassementId { get; set; }

        public int DeclassementId { get; set; }

        public string Agent { get; set; }

        public string TypeMouvement { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateMouvement { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime HeureMouvement { get; set; }

        public string NatureMouvement { get; set; }

        public string MotifDeclassement { get; set; }

        public string ProduitDeclasse { get; set; }

        public int QteDeclasse { get; set; }
    }
}
