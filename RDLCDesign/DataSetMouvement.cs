using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace RDLCDesign
{
    public partial class DataSetMouvement : DbContext
    {
        public DataSetMouvement()
            : base("name=DataSetMouvement")
        {
        }

        public virtual DbSet<DetailsMouvement> DetailsMouvements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
