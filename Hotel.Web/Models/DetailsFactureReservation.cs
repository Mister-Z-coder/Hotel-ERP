using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class DetailsFactureReservation
    {   
        public int DetailsFactureReservationId { get; set; }
        public int Numres { get; set; }            // ReservationId
        public int Numfact { get; set; }           // ReglementId
        public DateTime Datefact { get; set; }       // DateReglement
        public DateTime Heurefact { get; set; }      // HeureReglement
        public DateTime Debut { get; set; }       //Debut occupation
        public DateTime Fin { get; set; }           //Fin occupation
        public int Caissier { get; set; }          // AgentId
        public int Caisse { get; set; }            // CaisseId
        public string Chambre { get; set; }    // ProduitNom
        public int Duree { get; set; }               // Duree
        public decimal Montant { get; set; }             // Montant
        public string ModeReglement { get; set; }  // TitreModeReglement
        public string Service { get; set; }  // Service
        public string TypeReglement { get; set; }  // TitreTypeReglement
        public string Devise { get; set; }         // SigleDevise
    }
}
