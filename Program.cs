using BiblioApp.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<HomeService>();
builder.Services.AddScoped<HomeService>();

builder.Services.AddHttpClient<LibroService>();
builder.Services.AddScoped<LibroService>();

builder.Services.AddHttpClient<PrestamoService>();
builder.Services.AddScoped<PrestamoService>();

builder.Services.AddHttpContextAccessor(); 


var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
