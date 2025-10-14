using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class PointdeVenteViewModel
    {
        //public Commande Commande { get; set; }
        public List<Agent> Serveurs { get; set; }
        public List<Aliment> Aliments { get; set; }
        public List<Boisson> Boissons { get; set; }
        public List<Client> Clients { get; set; }
        public List<Service> Services { get; set; }
        public List<TableClient> TableClients { get; set; }
        public List<CommandeJour> CommandeJours { get; set; }
    }
}
