using DentaMatch.Data;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Repository.Authentication;
using Microsoft.EntityFrameworkCore;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Repository.Dental_Case;
using DentaMatchAdmin.Services.Calculations.IServices;
using DentaMatchAdmin.Services.Calculations;
using DentaMatch.Models;
using Microsoft.AspNetCore.Identity;
using DentaMatchAdmin.Cache;
using DentaMatchAdmin.Services.DoctorVerification.IServices;
using DentaMatchAdmin.Services.DoctorVerification;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Authentication;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.Services.Mail;
using DentaMatch.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<AppHelper>();
builder.Services.AddScoped<CacheItem>();
builder.Services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();
builder.Services.AddScoped<IDentalUnitOfWork, DentalUnitOfWork>();

builder.Services.AddScoped<IHomePageService, HomePageService>();
builder.Services.AddScoped<IDoctorVerificationService, DoctorVerificationService>();
builder.Services.AddScoped<IAuthDoctorService, AuthDoctorService>();
builder.Services.AddTransient<IMailService, MailService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();