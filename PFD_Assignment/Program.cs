//using Microsoft.AspNetCore.Authentication.Cookies;
//using Google.Apis.Auth.AspNetCore3;
using System.Reflection;
using PFD_Assignment.Models;
using Microsoft.EntityFrameworkCore;
using PFD_Assignment.DAL;

var builder = WebApplication.CreateBuilder(args);
var configurationBuilder = new ConfigurationBuilder();
var OpenAIKey = builder.Configuration["OpenAI:apikey"];

// Add a default in-memory implementation of distributed cache
builder.Services.AddDistributedMemoryCache();
// Add the session service
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ImgDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString
        ("SGHandbookConnectionString"));
});

// This configures Google.Apis.Auth.AspNetCore for use in this app.
/* will work on it ltr
builder.Services.AddAuthentication(options =>
{
    // Login (Challenge) to be handled by Google OpenID Handler, 
    options.DefaultChallengeScheme =
    GoogleOpenIdConnectDefaults.AuthenticationScheme;
    // Once a user is authenticated, the OAuth2 token info 
    // is stored in cookies.
    options.DefaultScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogleOpenIdConnect(options =>
{
    // Credentials (stored in appsettings.json) to identify
    // the web app when performing Google authentication
    options.ClientId =
    builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret =
    builder.Configuration["Authentication:Google:ClientSecret"];
});*/
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
