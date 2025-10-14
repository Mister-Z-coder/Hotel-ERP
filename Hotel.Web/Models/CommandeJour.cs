using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class CommandeJour : IEquatable<CommandeJour>

    {
        public int CommandeId { get; set; }
        public string DtHrCommande { get; set; }
        public string TypeCommande { get; set; }
        public string ClientNom { get; set; }
        public string AgentNom { get; set; }
        public string TableTitre { get; set; }

        public bool Equals(CommandeJour other)
        {
            if (other == null) return false;

            return CommandeId == other.CommandeId &&
                   DtHrCommande == other.DtHrCommande &&
                   ClientNom == other.ClientNom &&
                   TableTitre == other.TableTitre &&
                   TypeCommande == other.TypeCommande &&
                   AgentNom == other.AgentNom;
        }

        public override int GetHashCode()
        {
            // Combinez les hash codes de toutes les propriétés comparées
            return HashCode.Combine(CommandeId, DtHrCommande, ClientNom, TableTitre, TypeCommande, AgentNom);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CommandeJour);
        }
    }

}
