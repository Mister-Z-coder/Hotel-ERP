using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class DemandeViewModel
    {
        public Demande Demandes { get; set; }
        public IEnumerable<Demande> Demande { get; set; }
        public string? SearchType { get; set; }
        public string? SearchDate { get; set; }
        public string? SearchEtat { get; set; }
        public string? SearchAgent { get; set; }
        public IEnumerable<SelectListItem> AgentList { get; set; }


        public IEnumerable<SelectListItem> ComprendreList { get; set; }
        public IEnumerable<SelectListItem> ComporterList { get; set; }
        public IEnumerable<Comprendre> Comprendre { get; set; }
        public IEnumerable<Comporter> Comporter { get; set; }

        public IEnumerable<Boisson> Boisson { get; set; }
        public IEnumerable<Aliment> Aliment { get; set; }

        //public Fournisseur Fournisseurs { get; set; }
        //public TypeMouvement TypeMouvements { get; set; }
        public Livraison Livraisons { get; set; }
        public SortiePourAjout SortiePourAjouts { get; set; }

        public IEnumerable<SelectListItem> FournisseurList { get; set; }
        public IEnumerable<SelectListItem> TypeMouvementList { get; set; }

    }
}
