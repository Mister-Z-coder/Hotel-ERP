using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace RDLCDesign
{
    public partial class DatasetDemande : DbContext
    {
        public DatasetDemande()
            : base("name=DatasetDemande")
        {
        }

        public virtual DbSet<DetailsDemande> DetailsDemandes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
