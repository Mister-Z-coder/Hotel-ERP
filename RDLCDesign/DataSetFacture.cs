using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace RDLCDesign
{
    public partial class DataSetFacture : DbContext
    {
        public DataSetFacture()
            : base("name=DataSetFacture")
        {
        }

        public virtual DbSet<DetailsFacture> DetailsFactures { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
