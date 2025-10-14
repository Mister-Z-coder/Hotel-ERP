using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Hotel.Services
{
    public class CaisseRedirectFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //Verifier si l'utilisateur est connecté
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                //Verifier si l'utilisateur est un caissier
                if (context.HttpContext.User.IsInRole("CAISSIER"))
                {
                    //Rediriger vers le point de vente (caisse)
                    context.Result = new RedirectToActionResult("Index", "PointdeVente", null);
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
