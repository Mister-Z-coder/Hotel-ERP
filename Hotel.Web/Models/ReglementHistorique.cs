using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ReglementHistorique
    {
        public int ReglementHistoriqueId { get; set; }
        public int ReglementId { get; set; }

        public int ModeReglementId { get; set; }
        public int DeviseId { get; set; }
        public int TypeReglementId { get; set; }
        public int AgentId {get;set;}
        public int CaisseId { get; set; }
        public DateTime DateReglement { get; set; }
        public DateTime HeureReglement { get; set; }
        public decimal MontantReglement { get; set; }
        public string MotifReglement { get; set; }

    }
}
