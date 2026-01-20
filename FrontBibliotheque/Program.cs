using FrontBibliotheque.Data;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri("http://localhost:5232/");
});

builder.Services.AddScoped<UtilisateurRepository>();
builder.Services.AddScoped<HistoriqueAbonnementRepository>();
builder.Services.AddScoped<V_livreRepository>();
// 🔥 SESSION
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // utile en HTTP
});

var app = builder.Build();

// ================= PIPELINE =================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseRouting();

// 🔥 OBLIGATOIRE : AVANT MapControllers
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

// MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// API
app.MapControllers();
app.UseStaticFiles();


app.Run();
