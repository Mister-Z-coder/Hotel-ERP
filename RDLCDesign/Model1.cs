using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace RDLCDesign
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Hotel")
        {
        }

        public virtual DbSet<DetailsFacture> DetailsFactures { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
