using DotNetEnv;
using InventarioInteligenteBack.Infrastructure.Identity;
using InventarioInteligenteBack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// 1. Conexión a BD
var connectionString = Env.GetString("CONNECTIONSTRINGS__DEFAULT");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(connectionString));

// 2. Identity
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// 3. Configuración JWT simple
var jwtKey = builder.Configuration["JWT:KEY"] ?? "claveSuperSecreta1234567890123456"; // fallback
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddScoped<
    InventarioInteligenteBack.Application.Interfaces.IProductoService,
    InventarioInteligenteBack.Application.Services.ProductoService>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // 👈 Importante: antes de Authorization
app.UseAuthorization();
app.MapControllers();
app.Run();
