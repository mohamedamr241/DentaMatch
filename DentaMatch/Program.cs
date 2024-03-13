using DentaMatch.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DentaMatch.Models;
using DentaMatch.Helpers;
using DentaMatch.Services.Cases_Appointment;
using DentaMatch.Services.Mail;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Repository.Authentication;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Repository.Dental_Case;
using DentaMatch.Services.Dental_Case.IServices;
using DentaMatch.Services.Authentication;
using DentaMatch.Services.Cases_Appointment.IServices;
using DentaMatch.Services.Dental_Case;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Paymob.Iservice;
using DentaMatch.Services.Paymob;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();



builder.Services.AddScoped<IDentalUnitOfWork, DentalUnitOfWork>();
builder.Services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();
builder.Services.AddScoped<AppHelper>();

builder.Services.AddScoped<IDentalCaseService, DentalCaseService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPaymobService, PaymobService>();
builder.Services.AddScoped<IAuthPatientService, AuthPatientService>();
builder.Services.AddScoped<IAuthAdminService, AuthAdminService>();
builder.Services.AddScoped<IAuthDoctorService, AuthDoctorService>();
builder.Services.AddScoped<ICaseAppointmentService, CaseAppointmentService>();


builder.Services.AddHttpContextAccessor();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();