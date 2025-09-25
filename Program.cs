using DotNetEnv;
using InventarioInteligenteBack.Infrastructure.Identity;
using InventarioInteligenteBack.Infrastructure.Persistence;
using InventarioInteligenteBack.Application.Interfaces;
using InventarioInteligenteBack.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using QuestPDF.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ⚡ Configurar licencia de QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

// Cargar variables de entorno
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
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;
});
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// 3. Configuración JWT
var jwtKey = builder.Configuration["JWT:KEY"] ?? "claveSuperSecreta1234567890123456";
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
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPaisService, PaisService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IImpuestoService, ImpuestoService>();
builder.Services.AddScoped<IDescuentoService, DescuentoService>();
builder.Services.AddScoped<IFacturaPdfService, FacturaPdfService>();
builder.Services.AddScoped<IFacturaService, FacturaService>();

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

// 👇 Crear carpeta de facturas si no existe
var facturasPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "storage", "facturas");
if (!Directory.Exists(facturasPath))
    Directory.CreateDirectory(facturasPath);

// 👇 Configurar content-type provider (PDF asegurado)
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".pdf"] = "application/pdf";

// 👇 Archivos estáticos (todo wwwroot)
app.UseStaticFiles();

// 👇 Archivos estáticos para facturas, con MIME forzado
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(facturasPath),
    RequestPath = "/storage/facturas",
    ContentTypeProvider = provider,
    ServeUnknownFileTypes = true
});

// 👇 Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
