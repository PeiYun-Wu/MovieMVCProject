using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using PennyProject.DataBase.MovieDB;
using PennyProject.Helpers;
using PennyProject.Repo;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .AddJsonFile("appsettings.Development.json")
              .Build();

#region db context
string conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
string connSec = configuration.GetConnectionString("ConnectionSec") ?? "";
string dbConfig = AESUtill.DecryptAES(conn, connSec);
builder.Services.AddDbContext<PennyMovieDBContext>(options =>
{
    options.UseSqlServer(dbConfig);
});
#endregion

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IFavService, FavService>();
builder.Services.AddHttpContextAccessor();

#region NLog
var nlogConfig = configuration.GetSection("NLog");
LogManager.Configuration = new NLogLoggingConfiguration(nlogConfig);

builder.Logging.ClearProviders();

var nlogOptions = new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = false };
builder.Host.UseNLog(nlogOptions);
#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Movie/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

//first show controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Index}/{id?}");

app.Run();
