using System;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using NerdDinner.Web.Data;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web
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


            services.AddDbContext<ApplicationDbContext>(option => option.UseMySql("server=localhost;database=DinnerApp;username=root;password=;"));
            services.AddTransient<INerdDinnerRepository, NerdDinnerRepository>();

            // Add Identity services to the services container
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();


            services.AddControllersWithViews();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("http://example.com");
                });
            });

            services.AddLogging();



            //// Add memory cache services
            //if (HostingEnvironment.IsProduction())
            //{
            //    services.AddMemoryCache();
            //    services.AddDistributedMemoryCache();
            //}

            // Add session related services.
            // TODO: Test Session timeout
            services.AddSession(options =>
            {
                // options.CookieName = ".AdventureWorks.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(10);
            });

            // Add the system clock service
            services.AddSingleton<ISystemClock, SystemClock>();

            // Configure Auth
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "ManageDinner",
                    authBuilder =>
                    {
                        authBuilder.RequireClaim("ManageDinner", "Allowed");
                    });
            });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = "327959782619-ergtgk0e44h1bi76u7irt8jg7oa1a1vc.apps.googleusercontent.com";
                googleOptions.ClientSecret = "0mnFB12WFXelPkR0PXkC2vrC";
            }).AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = "5609270052582677";
                facebookOptions.AppSecret = "3d9a853452f18ca5e928e96602307525";
            }).AddTwitter(twitterOptions =>
            {
                twitterOptions.ConsumerKey = "lDSPIu480ocnXYZ9DumGCDw37";
                twitterOptions.ConsumerSecret = "fpo0oWRNc3vsZKlZSq1PyOSoeXlJd7NnG4Rfc94xbFXsdcc3nH";
            });
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
          
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
             

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            SampleData.InitializeNerdDinner(app.ApplicationServices).Wait();
        }
    }
}
