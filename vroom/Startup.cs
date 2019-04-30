using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using vroom.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace vroom
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            }); 
            services.AddDbContext<VroomDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<VroomDbContext>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();
            
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "ByYearMonth",
                    "make/bikes/{year:int:length(4)}/{month:int:range(1,12)}",
                    new {controller="make", action="ByYearMonth"},
                    new {year=@"2017|2018"}
                    );

                  routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"                    
                );
            });
        }
    }
}
