using ApiInteligenteTareas.Data;
using ApiInteligenteTareas.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Puerto para Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cadena de conexión compatible con Render
var connectionString = Environment.GetEnvironmentVariable("RENDER") == "true"
    ? "Data Source=/tmp/tareas.db"
    : builder.Configuration.GetConnectionString("DefaultConnection");

// Si se ejecuta local y usa carpeta Data, crearla si no existe
if (!string.IsNullOrWhiteSpace(connectionString) &&
    connectionString.Contains("Data Source=Data/", StringComparison.OrdinalIgnoreCase))
{
    var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");

    if (!Directory.Exists(dataDirectory))
    {
        Directory.CreateDirectory(dataDirectory);
    }
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddHttpClient<ITareaExternaService, TareaExternaService>(client =>
{
    var baseUrl = builder.Configuration["ExternalApis:JsonPlaceholderBaseUrl"]
                  ?? "https://jsonplaceholder.typicode.com/";

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddSingleton<ISentimientoService, SentimientoService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("PermitirTodo");

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();