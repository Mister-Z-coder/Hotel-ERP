using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace RDLCDesign
{
    public partial class DatasetLivraison : DbContext
    {
        public DatasetLivraison()
            : base("name=DatasetLivraison")
        {
        }

        public virtual DbSet<DetailsLivraison> DetailsLivraisons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
