var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

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
    defaults: new { controller = "Products", action = "Cart" }
);
app.MapControllerRoute(
    name: "Detail",
    pattern: "/detail",
    defaults: new { controller = "Products", action = "Detail" }
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
app.Run();
