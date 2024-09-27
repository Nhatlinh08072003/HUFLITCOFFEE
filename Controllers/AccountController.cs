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
        [HttpGet]
        public async Task<IActionResult> Profile()
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
                return NotFound(); // Trả về 404 nếu không tìm thấy người dùng
            }

            var profileViewModel = new ProfileViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
                // Thêm các thuộc tính khác nếu cần
            };

            return View(profileViewModel); // Đảm bảo rằng bạn đang trả về view model
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //       [HttpPost]
        // public async Task<IActionResult> Login(LoginViewModel model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var user = await _huflitcoffeeContext.Users
        //             .FirstOrDefaultAsync(u => u.Username == model.Username && u.PasswordHash == model.Password);

        //         if (user != null && user.Username != null && user.Email != null)
        //         {
        //             var claims = new List<Claim>
        //             {
        //                 new Claim(ClaimTypes.Name, user.Username),
        //                 new Claim(ClaimTypes.Email, user.Email),
        //                 new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
        //             };



        //             var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //             await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        //             ViewBag.Username = user.Username;
        //             return RedirectToAction("Index", "Home");
        //         }

        //         ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác. Vui lòng thử lại.");
        //     }

        //     return View(model);
        // }
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
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),

            };
                    // Chỉ thêm Claim cho Role nếu Role không phải là null
                    if (!string.IsNullOrEmpty(user.Role))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, user.Role));
                    }
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    ViewBag.Username = user.Username;

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin"); // Chuyển hướng đến trang admin
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chủ
                    }
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

                    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("localDB")))
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
                            command.Parameters.AddWithValue("@CreatedAt", TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "SE Asia Standard Time"));
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
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("localDB")))
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

        [HttpGet]
        public IActionResult Resetpassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Lấy UserId từ claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return NotFound();
                }
                var userId = int.Parse(userIdClaim.Value);

                // Lấy thông tin người dùng từ database
                var user = await _huflitcoffeeContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound(); // Return a 404 if user is not found
                }

                // Kiểm tra mật khẩu hiện tại
                if (user.PasswordHash != model.CurrentPassword)
                {
                    ModelState.AddModelError(nameof(model.CurrentPassword), "Mật khẩu hiện tại không đúng.");
                    return View(model);
                }

                // Cập nhật mật khẩu mới
                user.PasswordHash = model.NewPassword;
                _huflitcoffeeContext.Users.Update(user);
                await _huflitcoffeeContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ sau khi đổi mật khẩu thành công
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
