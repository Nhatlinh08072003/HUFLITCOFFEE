using HUFLITCOFFEE.web.Data;
using HUFLITCOFFEE.Models.Main;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using HUFLITCOFFEE.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HuflitcoffeeContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("localDB"));
    options.EnableSensitiveDataLogging(false);
});
builder.Services.AddSingleton<ITimeZoneService, TimeZoneService>();

// Cấu hình xác thực
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Ví dụ: Thiết lập thời gian hết hạn
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true; // Ví dụ: Bật thời gian hết hạn trượt
    });

// Cấu hình phân quyền
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddControllersWithViews();

// Cấu hình session
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Cần thiết cho tuân thủ GDPR
});

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
};
builder.Services.AddSingleton<IVnPayService, VnPayService>();

var app = builder.Build();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Caffee",
    pattern: "/caffee",
    defaults: new { controller = "Home", action = "Caffee" }
);
app.MapControllerRoute(
    name: "Product",
    pattern: "/product",
    defaults: new { controller = "Home", action = "Product" }
);
app.MapControllerRoute(
    name: "Share",
    pattern: "/share",
    defaults: new { controller = "Home", action = "Share" }
);
app.MapControllerRoute(
    name: "Recruitment",
    pattern: "/recruitment",
    defaults: new { controller = "Home", action = "Recruitment" }
);
app.MapControllerRoute(
    name: "Aboutus",
    pattern: "/aboutus",
    defaults: new { controller = "Home", action = "Aboutus" }
);
app.MapControllerRoute(
    name: "Job",
    pattern: "/job",
    defaults: new { controller = "Home", action = "Job" }
);
app.MapControllerRoute(
    name: "Translation",
    pattern: "/translation",
    defaults: new { controller = "Home", action = "Translation" }
);
app.MapControllerRoute(
    name: "Roots",
    pattern: "/roots",
    defaults: new { controller = "Home", action = "Roots" }
);
app.MapControllerRoute(
    name: "Policy",
    pattern: "/policy",
    defaults: new { controller = "Home", action = "Policy" }
);
app.MapControllerRoute(
    name: "Terms",
    pattern: "/terms",
    defaults: new { controller = "Home", action = "Terms" }
);
app.MapControllerRoute(
    name: "Customer",
    pattern: "/customer",
    defaults: new { controller = "Admin", action = "Customer" }
);
app.MapControllerRoute(
    name: "Order",
    pattern: "/order",
    defaults: new { controller = "Admin", action = "Order" }
);
app.MapControllerRoute(
    name: "Overview",
    pattern: "/overview",
    defaults: new { controller = "Admin", action = "Overview" }
);
app.MapControllerRoute(
    name: "Productlist",
    pattern: "/productlist",
    defaults: new { controller = "Admin", action = "Productlist" }
);
app.MapControllerRoute(
    name: "Shipping",
    pattern: "/shipping",
    defaults: new { controller = "Admin", action = "Shipping" }
);
app.MapControllerRoute(
    name: "Staff",
    pattern: "/staff",
    defaults: new { controller = "Admin", action = "Staff" }
);
app.MapControllerRoute(
    name: "Cart",
    pattern: "/cart",
    defaults: new { controller = "Product", action = "Cart" }
);
app.MapControllerRoute(
    name: "Detail",
    pattern: "/detail/{id}",
    defaults: new { controller = "Product", action = "Detail" }
);
app.MapControllerRoute(
    name: "Products",
    pattern: "/products",
    defaults: new { controller = "Product", action = "Products" }
);
app.MapControllerRoute(
    name: "Sale",
    pattern: "/sale",
    defaults: new { controller = "Product", action = "Sale" }
);
app.MapControllerRoute(
    name: "Shipping",
    pattern: "/shipping",
    defaults: new { controller = "Product", action = "Shipping" }
);
// app.MapControllerRoute(
//     name: "PaymentSuccess",
//     pattern: "/paymentsuccess",
//     defaults: new { controller = "Product", action = "PaymentSuccess" }
// );
// app.MapControllerRoute(
//     name: "PaymentFail",
//     pattern: "/paymentfail",
//     defaults: new { controller = "Product", action = "PaymentFail" }
// );
app.MapControllerRoute(
    name: "Historybuy",
    pattern: "/historybuy",
    defaults: new { controller = "Account", action = "Historybuy" }
);
app.MapControllerRoute(
    name: "Login",
    pattern: "/login",
    defaults: new { controller = "Account", action = "Login" }
);
app.MapControllerRoute(
    name: "Register",
    pattern: "/register",
    defaults: new { controller = "Account", action = "Register" }
);
app.MapControllerRoute(
    name: "Profile",
    pattern: "/profile",
    defaults: new { controller = "Account", action = "Profile" }
);
app.MapControllerRoute(
    name: "OrderHistory",
    pattern: "/orderhistory",
    defaults: new { controller = "Account", action = "OrderHistory" }
);
app.MapControllerRoute(
    name: "OrderDetailHistory",
    pattern: "/orderdetailhistory{id}",
    defaults: new { controller = "Account", action = "OrderDetailHistory" }
);
app.MapControllerRoute(
name: " Admin faffafaf",
pattern: "/admin",
defaults: new { controller = "Role", action = "Admin" }
);
app.MapControllerRoute(
name: " Forgotpassword faffafaf",
pattern: "/forgot",
defaults: new { controller = "Account", action = "Forgotpassword" }
);
app.MapControllerRoute(
name: "Resetpassword faffafaf",
pattern: "/resetpassword",
defaults: new { controller = "Account", action = "Resetpassword" }
);
app.MapControllerRoute(
name: " Role User faffafaf",
pattern: "/user",
defaults: new { controller = "Role", action = "Customer" }
);
app.Run();
