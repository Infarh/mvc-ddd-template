using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolutionTemplate.DAL.Sqlite;
using SolutionTemplate.DAL.SqlServer;

namespace SolutionTemplate.MVC
{
    public record Startup(IConfiguration Configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var db_type = Configuration["Database"];
            switch (db_type)
            {
                case "SqlServer":
                    services.AddSolutionTemplateDbContextSqlServer(Configuration.GetConnectionString(db_type));
                    break;

                case "Sqlite":
                    services.AddSolutionTemplateDbContextSqlite(Configuration.GetConnectionString(db_type));
                    break;
            }

            services.AddValidatorsFromAssembly(typeof(Startup).Assembly);

            services.AddSignalR();
            services.AddMediatR(typeof(Startup));
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
            });

            services.AddControllersWithViews()
               .AddRazorRuntimeCompilation();

            //services.AddHealthChecks()
               //.AddCheck<BaseHealthCheck>("Base");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseStatusCodePagesWithReExecute("~/Status/{0}");

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHealthChecks("/health");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
