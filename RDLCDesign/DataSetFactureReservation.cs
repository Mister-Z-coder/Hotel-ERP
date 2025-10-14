using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace RDLCDesign
{
    public partial class DataSetFactureReservation : DbContext
    {
        public DataSetFactureReservation()
            : base("name=DataSetFactureReservation")
        {
        }

        public virtual DbSet<DetailsFactureReservation> DetailsFactureReservations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
