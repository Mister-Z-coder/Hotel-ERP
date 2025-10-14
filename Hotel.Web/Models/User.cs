using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nom de l\'utilisateur requis")]
        [Display(Name = "Nom de l\'utilisateur ")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mot de passe de l\'utilisateur requis")]
        [Display(Name = "Mot de passe de l\'utilisateur ")]
        public string Password { get; set; } // En production, utilisez un hash pour le mot de passe

        [Required(ErrorMessage = "Role de l\'utilisateur requis")]
        [Display(Name = "Role de l\'utilisateur ")]
        public string Role { get; set; } // Pour gérer les rôles (admin, user, etc.)

        [Required(ErrorMessage = "Agent requis")]
        [Display(Name = "Agent ")]
        [ForeignKey("AgentId")]
        public int? AgentId { get; set; } // Clé étrangère (nullable)
        public Agent Agent { get; set; }
    }
}
