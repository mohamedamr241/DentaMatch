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
using DentaMatchAdmin.MiddleWares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHomePageService, HomePageService>();
builder.Services.AddScoped<IDoctorVerificationService, DoctorVerificationService>();
builder.Services.AddScoped<IAuthDoctorService, AuthDoctorService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Auth/SignIn";
    //tions.LogoutPath = $"/Identity/Account/Logout";
    //tions.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:key"]))
        };
    });

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
app.UseSession();

app.UseMiddleware<TokenMiddleWare>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();