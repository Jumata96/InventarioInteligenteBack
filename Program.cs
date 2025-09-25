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

// 👇 Política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")  
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


// 1. Conexión a BD
var connectionString = Env.GetString("CONNECTIONSTRINGS__DEFAULT");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(connectionString));

// 2. Identity
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    // Reglas de contraseña
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true; // caracter especial
    options.Password.RequiredLength = 8;

    // Bloqueo de cuenta
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;

    // Reglas de usuario
    options.User.RequireUniqueEmail = true;
});
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

// 4. Registro de servicios de aplicación
builder.Services.AddScoped<
    InventarioInteligenteBack.Application.Interfaces.IProductoService,
    InventarioInteligenteBack.Application.Services.ProductoService>();

builder.Services.AddScoped<
    InventarioInteligenteBack.Application.Interfaces.IClienteService,
    InventarioInteligenteBack.Application.Services.ClienteService>();

builder.Services.AddScoped<
    InventarioInteligenteBack.Application.Interfaces.IPaisService,
    InventarioInteligenteBack.Application.Services.PaisService>();

builder.Services.AddScoped<
    InventarioInteligenteBack.Application.Interfaces.IPedidoService,
    InventarioInteligenteBack.Application.Services.PedidoService>();

builder.Services.AddScoped<
    InventarioInteligenteBack.Application.Interfaces.IImpuestoService,
    InventarioInteligenteBack.Application.Services.ImpuestoService>();

// 5. API y controladores
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();


// 👇 Habilitar CORS
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // 👈 Importante: antes de Authorization
app.UseAuthorization();
app.MapControllers();
app.Run();
