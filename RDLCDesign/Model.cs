using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace RDLCDesign
{
    public partial class Model : DbContext
    {
        public Model()
            : base("name=Hotel")
        {
        }

        public virtual DbSet<DetailsCommandes> DetailsCommandes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
