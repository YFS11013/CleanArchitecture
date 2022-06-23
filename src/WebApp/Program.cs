using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Server.Extensions;
using BlazorHero.CleanArchitecture.Server.Filters;
using BlazorHero.CleanArchitecture.Server.Managers.Preferences;
using BlazorHero.CleanArchitecture.Server.Middlewares;
using Hangfire;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebApp.Areas.Identity;
using WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
using var host = Host.CreateDefaultBuilder(args).Build();
IConfiguration _configuration = host.Services.GetRequiredService<IConfiguration>();

builder.Services.AddForwarding(_configuration);
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.AddCurrentUserService();
builder.Services.AddSerialization();
builder.Services.AddDatabase(_configuration);
builder.Services.AddServerStorage(); //TODO - should implement ServerStorageProvider to work correctly!
builder.Services.AddScoped<ServerPreferenceManager>();
builder.Services.AddServerLocalization();
builder.Services.AddIdentity();
builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSettings(_configuration));
builder.Services.AddSignalR();
builder.Services.AddApplicationLayer();
builder.Services.AddApplicationServices();
builder.Services.AddRepositories();
builder.Services.AddExtendedAttributesUnitOfWork();
builder.Services.AddSharedInfrastructure(_configuration);
builder.Services.RegisterSwagger();
builder.Services.AddInfrastructureMappings();
//builder.Services.AddHangfire(x => x.UseSqlServerStorage(_configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddHangfireServer();
builder.Services.AddControllers().AddValidators();
builder.Services.AddExtendedAttributesValidators();
builder.Services.AddExtendedAttributesHandlers();
builder.Services.AddRazorPages();
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});
builder.Services.AddLazyCache();








//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<BlazorHeroUser>>();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();
//IConfiguration configuration = app.Configuration;

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseMigrationsEndPoint();
//}
//else
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();

//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();
//app.MapBlazorHub();
//app.MapFallbackToPage("/_Host");

//app.Run();


app.UseForwarding(_configuration);
app.UseExceptionHandling(app.Environment);
app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
    RequestPath = new PathString("/Files")
});
app.UseRequestLocalizationByCulture();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.UseHangfireDashboard("/jobs", new DashboardOptions
//{
//    DashboardTitle = "BlazorHero Jobs",
//    Authorization = new[] { new HangfireAuthorizationFilter() }
//});
app.UseEndpoints();
app.ConfigureSwagger();
app.Initialize(_configuration);

app.RunAsync();