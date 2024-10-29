using jbp_wapp.Data;
using jbp_wapp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuración de la base de datos (reemplaza la cadena de conexión con la tuya)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin() // Permite cualquier origen
                   .AllowAnyMethod() // Permite cualquier método (GET, POST, etc.)
                   .AllowAnyHeader(); // Permite cualquier cabecera
        });
});

// Configuración de sesión
builder.Services.AddSession(options =>
{
    // Tiempo de expiracion de la sesion
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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

// Habilitar CORS
app.UseCors("AllowAll");

// Habilitar autenticación y autorización
app.UseRouting();

// Middleware para la sesion
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
