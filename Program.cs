using BiblioApp.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 💡 Registrá HomeService
builder.Services.AddHttpClient<HomeService>();
builder.Services.AddScoped<HomeService>();

builder.Services.AddHttpClient<LibroService>();
builder.Services.AddScoped<LibroService>();

builder.Services.AddHttpClient<PrestamoService>();
builder.Services.AddScoped<PrestamoService>();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // no lo olvides

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();