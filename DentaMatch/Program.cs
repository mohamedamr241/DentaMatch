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
using DentaMatch.Services.Paypal.IServices;
using DentaMatch.Services.Paypal;
using DentaMatch.Cache;
using DentaMatch.Services.Reports;
using DentaMatch.Services.Reports.IService;
using DentaMatch.Services.Comments;
using DentaMatch.Services.Comments.IServices;
using DentaMatch.Middlewares;
using DentaMatch.Services.CaseProgress.IServices;
using DentaMatch.Services.CaseProgress;
using DentaMatch.Repository.Notifications.IRepository;
using DentaMatch.Repository.Notifications;
using DentaMatch.Services;
using DentaMatch.Services.FireBase.IServices;
using DentaMatch.Services.FireBase;
using DentaMatch.Services.Notifications.IServices;
using DentaMatch.Services.Notifications;
using DentaMatch.Services.Specialization.IServices;
using DentaMatch.Services.Specialization;
using DentaMatch.Repository.Specialization.IRepository;
using DentaMatch.Repository.Specialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IDentalUnitOfWork, DentalUnitOfWork>();
builder.Services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();

builder.Services.AddScoped<AppHelper>();
builder.Services.AddScoped<CacheItem>();

builder.Services.AddScoped<IDentalCaseService, DentalCaseService>();
builder.Services.AddScoped<IDentalCaseCommentRepository, DentalCaseCommentRepository>();
builder.Services.AddScoped<ICaseProgressService, CaseProgressService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPaymobService, PaymobService>();
builder.Services.AddScoped<IPaypalServices, PaypalServices>();
builder.Services.AddScoped<IAuthPatientService, AuthPatientService>();
builder.Services.AddScoped<IAuthAdminDoctorService, AuthAdminDoctorService>();
builder.Services.AddScoped<IAuthAdminService, AuthAdminService>();
builder.Services.AddScoped<IAuthDoctorService, AuthDoctorService>();
builder.Services.AddScoped<ICaseAppointmentService, CaseAppointmentService>();
builder.Services.AddScoped<ICaseCommentsService, CaseCommentsService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationServices>();
builder.Services.AddScoped<IFirebaseService, FirebaseService>();
builder.Services.AddScoped<ITableDependencyService, TableDependencyService>();
builder.Services.AddScoped<ISpecializationService, SpecializationService>();
builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();


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
app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();


app.UseWhen(context => context.Request.Path.StartsWithSegments("/Patient/DentalCase"), appBuilder =>
{
    appBuilder.UseMiddleware<BlockCheckMiddleware>();
});

app.UseWhen(context => (context.Request.Path.StartsWithSegments("/PatientAuth/GetAccount") || context.Request.Path.StartsWithSegments("/PatientAuth/UpdateAccount") || context.Request.Path.StartsWithSegments("/Patient/DentalCase") || context.Request.Path.StartsWithSegments("/DentalCase/addcomment") || context.Request.Path.StartsWithSegments("/Auth/DeleteAccount")), appBuilder =>
{
    appBuilder.UseMiddleware<BlockCheckMiddleware>();
});



app.UseWhen(context => context.Request.Path.StartsWithSegments("/Patient/CaseAppointment"), appBuilder =>
{
    appBuilder.UseMiddleware<BlockCheckMiddleware>();
});
app.UseSqlTableDependency(builder.Configuration.GetConnectionString("DefaultConnection"));


app.Run();