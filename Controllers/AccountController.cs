using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using HUFLITCOFFEE.Models.Main;
using HUFLITCOFFEE.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace HUFLITCOFFEE.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly HuflitcoffeeContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ILogger<AccountController> logger, HuflitcoffeeContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.PasswordHash == model.Password);

                if (user != null && user.Username != null && user.Email != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    ViewBag.Username = user.Username;
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác. Vui lòng thử lại.");
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account"); // Chuyển hướng về trang Login
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("/account/adduser")]
        public async Task<IActionResult> AddProduct(
     [FromForm] string fullname,
     [FromForm] string email,
     [FromForm] string psw,
     [FromForm] string username,
     [FromForm] string phone,
     [FromForm] string address)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("CoffeeDBConnectionString")))
                    {
                        await connection.OpenAsync();

                        string sql = @"
                    INSERT INTO [User] ( Username, PasswordHash, Email, FullName, Address, PhoneNumber, CreatedAt)
VALUES (@Username, @PasswordHash, @Email, @FullName, @Address, @PhoneNumber ,@CreatedAt)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@Username", username);
                            command.Parameters.AddWithValue("@PasswordHash", psw);
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@FullName", fullname);
                            command.Parameters.AddWithValue("@Address", address);
                            command.Parameters.AddWithValue("@PhoneNumber", phone);
                            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return Json(new { success = true, message = "Đăng ký thành công." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return Json(new { success = false, message = $"Lỗi khi lưu người dùng {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }
    }
}
