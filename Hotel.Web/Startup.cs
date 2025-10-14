using Hotel.Data;
using Hotel.Repository;
using Hotel.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hotel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth")
            .AddCookie("CookieAuth", options =>{
                //options.Cookie.HttpOnly = true;//Empeche l'accès aux cookies via javascript
                //options.Cookie.SecurePolicy = CookieSecurePolicy.;//Cookie envoyé que via HTTPS
                options.Cookie.SameSite = SameSiteMode.Strict;// Protège contre les attaques CSRF (Cross-Site Request Forgery).

                options.Cookie.Name = "UserLoginCookie";
            options.LoginPath = "/User/Login"; // Rediriger vers la page de login
            options.LogoutPath = "/User/Logout"; // Rediriger vers la page de logout
            options.AccessDeniedPath = "/User/AccessDenied"; // Rediriger en cas d'accès refusé
                options.Cookie.IsEssential = true; // Indiquer que le cookie est essentiel
                options.Cookie.MaxAge = null; // Aucun âge maximum, le cookie expire à la fermeture du navigateur
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            // Pour .NET Core 3.1+ et .NET 5+
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            services.AddAuthorization();
            services.AddScoped<UserService>();
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions => sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            ));
            services.AddOptions();
            services.AddTransient<IRepositoryDetailsCommande, DetailsCommandeRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();//Active HSTS
                //app.UseHttpsRedirection(); // Redirige HTTP vers HTTPS
            }

            //Désactivez les en-têtes HTTP qui pourraient révéler des informations sensibles sur votre serveur (par exemple, Server, X-Powered-By).

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");
                await next();
            });

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowAllOrigins");
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    //pattern: "{controller=User}/{action=Login}/{id?}");
                    pattern: "{controller=Home}/{action=Index}/{id?}");
        });
        }
    }
}
