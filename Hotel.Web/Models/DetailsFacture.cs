using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class DetailsFacture
    {   
        public int DetailsFactureId { get; set; }
        public int Numcmd { get; set; }            // CommandeId
        public int Numfact { get; set; }           // ReglementId
        public DateTime Datefact { get; set; }       // DateReglement
        public DateTime Heurefact { get; set; }      // HeureReglement
        public int Caissier { get; set; }          // AgentId
        public int Caisse { get; set; }            // CaisseId
        public string Designation { get; set; }    // ProduitNom
        public int Qte { get; set; }               // Quantite
        public decimal PU { get; set; }             // PrixUnitaire
        public string ModeReglement { get; set; }  // TitreModeReglement
        public string TypeReglement { get; set; }  // TitreTypeReglement
        public string Devise { get; set; }         // SigleDevise
        public decimal Montanttotal { get; set; }         // SigleDevise
    }
}
