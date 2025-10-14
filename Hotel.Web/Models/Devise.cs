using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Devise : IEquatable<Devise>
    {
        public int DeviseId { get; set; }

        [Required(ErrorMessage = "Titre de la devise requis")]
        [Display(Name = "Titre de la devise")]
        public string TitreDevise { get; set; }

        [Required(ErrorMessage = "Sigle de la devise requis")]
        [Display(Name = "Sigle de la devise")]
        public string SigleDevise { get; set; }

        public bool Equals(Devise other)
        {
            if (other == null) return false;

            return DeviseId == other.DeviseId &&
                   TitreDevise == other.TitreDevise &&
                   SigleDevise == other.SigleDevise;
        }

        public override int GetHashCode()
        {
            // Combinez les hash codes de toutes les propriétés comparées
            return HashCode.Combine(DeviseId, TitreDevise, SigleDevise);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Devise);
        }
    }
}
