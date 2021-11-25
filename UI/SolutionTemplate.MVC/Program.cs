using FluentValidation;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.DAL.Sqlite;
using SolutionTemplate.DAL.SqlServer;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

switch (builder.Configuration["Database"])
{
    case "SqlServer":
        services.AddSolutionTemplateDbContextSqlServer(builder.Configuration.GetConnectionString(builder.Configuration["Database"]));
        break;

    case "Sqlite":
        services.AddSolutionTemplateDbContextSqlite(builder.Configuration.GetConnectionString(builder.Configuration["Database"]));
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
