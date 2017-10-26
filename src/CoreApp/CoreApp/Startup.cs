using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;


namespace CoreApp
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
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                auth.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                
            })
           .AddCookie()
         .AddOpenIdConnect(options =>
         {
              options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
              options.Authority = Configuration["Authentication:KeycloakAuthentication:ServerAddress"] + "/auth/realms/" + Configuration["Authentication:KeycloakAuthentication:Realm"];
              options.RequireHttpsMetadata = false; //only in development
                                                    //PostLogoutRedirectUri = Configuration["Authentication:KeycloakAuthentication:PostLogoutRedirectUri"],
                                                    //SignedOutCallbackPath = Configuration["Authentication:KeycloakAuthentication:SignedOutCallbackPath"],
              options.ClientId = Configuration["Authentication:KeycloakAuthentication:ClientId"];
              options.ClientSecret = Configuration["Authentication:KeycloakAuthentication:ClientSecret"];
              options.ResponseType = OpenIdConnectResponseType.Code;
              options.GetClaimsFromUserInfoEndpoint = true;
              options.SaveTokens = true;
          });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Accounting", policy =>
            //    policy.RequireClaim("member_of", "[accounting]")); //this claim value is an array. Any suggestions how to extract just single role? This still works.
            //});

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
         
        }
    }
}
