using Buddget.BLL.Mappers;
using Buddget.BLL.Services.Implementations;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.DataAccess;
using Buddget.DAL.Repositories.Implementations;
using Buddget.DAL.Repositories.Interfaces;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

string connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

// Ensure that the connection string is not null or empty
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("The connection string is not defined.");
}

// Register the DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ��������� ����������
builder.Services.AddScoped<IFinancialSpaceRepository, FinancialSpaceRepository>();
builder.Services.AddScoped<IFinancialSpaceMemberRepository, FinancialSpaceMemberRepository>();
builder.Services.AddScoped<IFinancialGoalSpaceRepository, FinancialGoalSpaceRepository>();
builder.Services.AddScoped<IFinancialGoalRepository, FinancialGoalRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IFinancialSpaceService, FinancialSpaceService>();

// Add mappers
builder.Services.AddAutoMapper(typeof(FinancialSpaceProfile));

// Add logger
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{area=User}/{controller=FinancialSpace}/{action=Index}/{id?}",
    defaults: new { area = "User" });

app.Run();