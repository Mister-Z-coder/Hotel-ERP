namespace RDLCDesign
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DetailsMouvement
    {
        public int DetailsMouvementId { get; set; }

        public int MouvementId { get; set; }

        public string Agent { get; set; }

        public string TypeMouvement { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateMouvement { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime HeureMouvement { get; set; }

        public string NatureMouvement { get; set; }

        public string ProduitMouvement { get; set; }

        public int QteMouvement { get; set; }
    }
}
