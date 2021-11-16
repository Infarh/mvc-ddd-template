using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.DAL.Sqlite;
using SolutionTemplate.DAL.SqlServer;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

var db_type = configuration["Database"];
switch (db_type)
{
    case "SqlServer":
        services.AddSolutionTemplateDbContextSqlServer(configuration.GetConnectionString(db_type));
        break;

    case "Sqlite":
        services.AddSolutionTemplateDbContextSqlite(configuration.GetConnectionString(db_type));
        break;
}

services.AddValidatorsFromAssembly(typeof(Program).Assembly);
services.AddSignalR();
services.AddMediatR(typeof(Program));

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

var app = builder.Build();
await using (var scope = app.Services.CreateAsyncScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await initializer.InitializeAsync();
}


if (app.Environment.IsDevelopment())
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
