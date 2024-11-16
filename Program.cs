using jbp_wapp.Data;
using jbp_wapp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuraci�n de la base de datos (reemplaza la cadena de conexi�n con la tuya)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuraci�n de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin() // Permite cualquier origen
                   .AllowAnyMethod() // Permite cualquier m�todo (GET, POST, etc.)
                   .AllowAnyHeader(); // Permite cualquier cabecera
        });
});

// Agrega el esquema de autenticación de cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Redirige a Login si el usuario no está autenticado
        options.LogoutPath = "/Account/Logout"; // Ruta para cerrar sesión
        options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta para acceso denegado, si la configuras
    });

// Configuraci�n de sesi�n
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

// Middleware para la sesion
app.UseSession();

// Habilitar autenticaci�n y autorizaci�n
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
