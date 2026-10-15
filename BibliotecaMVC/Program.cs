using BibliotecaMVC.Repositories;
using BibliotecaMVC.Services;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BibliotecaContext>(Options =>
    Options.UseSqlServer(
        builder.Configuration.GetConnectionString("BibliotecaDB"))
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(Options =>
{
    Options.Lockout.MaxFailedAccessAttempts = 5;
    Options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    Options.Lockout.AllowedForNewUsers = true;

}).AddEntityFrameworkStores<BibliotecaContext>();

builder.Services.ConfigureApplicationCookie(Options =>
{
    Options.LoginPath = "/Account/Login";
    Options.AccessDeniedPath = "/Account/AccesDenied";
});


// Add services to the container.


builder.Services.AddSingleton<IRepositorioLibro, RepositorioEnMemoria>();
builder.Services.AddScoped<IAutorService, AutorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
