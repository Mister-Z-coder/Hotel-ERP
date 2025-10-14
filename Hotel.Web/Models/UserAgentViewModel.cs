using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class UserAgentViewModel
    {
        public User User { get; set; }
        [Display(Name ="Nouveau mot de passe")]
        public string? NewPassword { get; set; }
        public IEnumerable<SelectListItem> AgentList { get; set; }
    }
}
