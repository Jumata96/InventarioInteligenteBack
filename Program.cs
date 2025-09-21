using DotNetEnv;
using InventarioInteligente.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

var connectionString = Env.GetString("CONNECTIONSTRINGS__DEFAULT");

//builder.Services.AddDbContext<AppDbContext>(opt =>
//    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(connectionString));

builder.Services.AddScoped<
    InventarioInteligente.Application.Interfaces.IProductoService,
    InventarioInteligente.Application.Services.ProductoService>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   // /openapi/v1.json
    //app.MapSwaggerUi(); // /swagger
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
