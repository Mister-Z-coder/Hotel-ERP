using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace RDLCDesign
{
    public partial class DatasetDetailsSortie : DbContext
    {
        public DatasetDetailsSortie()
            : base("name=DatasetDetailsSortie")
        {
        }

        public virtual DbSet<DetailsSorty> DetailsSorties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
