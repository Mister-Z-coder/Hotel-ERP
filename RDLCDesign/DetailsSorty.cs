namespace RDLCDesign
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DetailsSorty
    {
        [Key]
        public int DetailsSortieId { get; set; }

        public int SortieId { get; set; }

        public string Agent { get; set; }

        public string TypeMouvement { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateMouvement { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime HeureMouvement { get; set; }

        public string NatureMouvement { get; set; }

        public int AjoutId { get; set; }

        public string MotifAjout { get; set; }

        public string ProduitLivre { get; set; }

        public int QteLivre { get; set; }
    }
}
