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
        private readonly HuflitcoffeeContext _huflitcoffeeContext;
        private readonly IConfiguration _configuration;

        public AccountController(ILogger<AccountController> logger, HuflitcoffeeContext context, IConfiguration configuration)
        {
            _logger = logger;
            _huflitcoffeeContext = context;
            _configuration = configuration;
        }
        public IActionResult Profile()
        {
            return View();
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
                var user = await _huflitcoffeeContext.Users
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
        public async Task<IActionResult> AddUser(
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
        [HttpGet]
        public IActionResult Forgotpassword()
        {
            return View();
        }
        public async Task<IActionResult> OrderHistory()
        {
            try
            {
                // Lấy UserId từ claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng." });
                }
                var userId = int.Parse(userIdClaim.Value);

                // Lấy các mục giỏ hàng của người dùng tương ứng
                var orders = await _huflitcoffeeContext.Orders
                                .Where(c => c.UserId == userId)
                                .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = $"Lỗi khi lấy giỏ hàng: {ex.Message}" });
            }
        }
        [Route("OrderDetailHistory/{id}")]
        public async Task<IActionResult> OrderDetailHistory(int id)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("CoffeeDBConnectionString")))
            {
                await connection.OpenAsync();

                // Lấy thông tin đơn hàng
                string orderSql = @"
            SELECT OrderID, UserID, FullName, Address, PhoneNumber, Total, Status, DateOrder, Ghichu
            FROM [Order]
            WHERE OrderID = @OrderID";

                Order? order = null;

                using (SqlCommand orderCommand = new SqlCommand(orderSql, connection))
                {
                    orderCommand.Parameters.AddWithValue("@OrderID", id);

                    using (SqlDataReader reader = await orderCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            order = new Order
                            {
                                OrderId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                FullName = reader.GetString(2),
                                Address = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                Total = reader.GetDecimal(5),
                                Status = reader.GetString(6),
                                DateOrder = reader.GetDateTime(7),
                                Ghichu = reader.GetString(8)
                            };
                        }
                    }
                }

                if (order == null)
                {
                    return NotFound();
                }

                // Lấy thông tin các sản phẩm trong giỏ hàng liên quan đến đơn hàng này
                string cartItemSql = @"
            SELECT ci.CartItemID, ci.UserID, ci.ProductID, ci.ImgProduct, ci.NameProduct, ci.PriceProduct, ci.Quantity, ci.ToppingNames, ci.DVT, ci.UnitPrice
            FROM CartItem ci
            INNER JOIN [Order] o ON o.UserID = ci.UserID
            WHERE o.OrderID = @OrderID";

                List<CartItem> cartItems = new List<CartItem>();

                using (SqlCommand cartItemCommand = new SqlCommand(cartItemSql, connection))
                {
                    cartItemCommand.Parameters.AddWithValue("@OrderID", id);

                    using (SqlDataReader reader = await cartItemCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            CartItem cartItem = new CartItem
                            {
                                CartItemId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                ProductId = reader.GetInt32(2),
                                ImgProduct = reader.GetString(3),
                                NameProduct = reader.GetString(4),
                                PriceProduct = reader.GetDecimal(5),
                                Quantity = reader.GetInt32(6),
                                ToppingNames = reader.GetString(7),
                                Dvt = reader.GetString(8),
                                UnitPrice = reader.GetDecimal(9)
                            };
                            cartItems.Add(cartItem);
                        }
                    }
                }

                // Tạo view model cho OrderDetail
                var orderDetailViewModel = new OrderDetailViewModel
                {
                    OrderID = order.OrderId,
                    FullName = order.FullName,
                    Address = order.Address,
                    PhoneNumber = order.PhoneNumber,
                    Total = order.Total,
                    Status = order.Status,
                    DateOrder = order.DateOrder,
                    Ghichu = order.Ghichu,
                    CartItems = cartItems
                };

                return View(orderDetailViewModel);
            }
        }
        public async Task<IActionResult> UserProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return NotFound();
            }

            var userId = int.Parse(userIdClaim.Value);

            var user = await _huflitcoffeeContext.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound(); // Return a 404 if user is not found
            }

            var profileViewModel = new ProfileViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt
                // Add other properties as needed
            };

            return View(profileViewModel);
        }



    }
}
