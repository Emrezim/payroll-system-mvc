using bordro.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<IdentityContext>(options =>
{
    var config = builder.Configuration;//appsettins.jsondaki verileri alır.(Geliştirme aşamasında appsettings.Develoment.json dan veri alınır.)
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLserverEmre"));
});
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();//Identityye User ve Role ile context eklenir.
builder.Services.Configure<IdentityOptions>(options =>
{
    //Kullanıcı için gerekli olan özelliklerin yeterliliklerinin ayarlanması.

    //Şifre için
    options.Password.RequiredLength = 1; // Gerekli olan en kısa uzunluk
    options.Password.RequireNonAlphanumeric = false;//Özel karakter gereksinimi
    options.Password.RequireLowercase = false;//Küçük harf gereksinimi
    options.Password.RequireUppercase = false;//Büyük harf gereksinimi
    options.Password.RequireDigit = false;//Sayı gereksinimi

    //Kullanıcı için
    options.User.RequireUniqueEmail = false;//Benzersiz başka bir deyişle başkalarında olmayan email gereksinimi(username ile kayıt olmak çin flase yap)
    //Hesap için
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //Başarısız hesap açma istekleri sonucunda hesabın kaç dakika kitleneceği
    options.Lockout.MaxFailedAccessAttempts = 100;//Hesabın kitlenmesi için gereken yanlış hesap açma isteği sayısı
    options.Lockout.AllowedForNewUsers = true;//Hesap kitleme sisteminin yeni kullanıcılarda gerek olup olmadığı

    //Giriş için
    // options.SignIn.RequireConfirmedEmail = true;//Giriş yapmak için onaylı hesap gereksinimi

});

builder.Services.ConfigureApplicationCookie(options =>
{
    //Çerez özelliklerinin ayarlanması
    options.LoginPath = "/Account/Login";//Giriş sayfasının yolu
    options.AccessDeniedPath = "/Account/Login";
    // options.AccessDeniedPath=""; //Eğer erişim reddedilirse yönlendirileceği sayfanın yolu(Bu projede gerek yoktu)
    options.SlidingExpiration = true;//Çerezin süresi geçmeden kullanıcı siteye girerse çerezin iptal süresini sıfırlar
    options.ExpireTimeSpan = TimeSpan.FromDays(10);//Çerezin iptal süresi

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

app.UseRouting();

app.UseAuthentication();
//auth altta olcak
//-----
app.UseAuthorization();
//----
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
await IdentitySeedData.IdentityTestUser(app);
app.Run();
