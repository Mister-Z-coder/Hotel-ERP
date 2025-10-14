using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace RDLCDesign
{
    public partial class DataSetDeclassement : DbContext
    {
        public DataSetDeclassement()
            : base("name=DataSetDeclassement")
        {
        }

        public virtual DbSet<DetailsDeclassement> DetailsDeclassements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
