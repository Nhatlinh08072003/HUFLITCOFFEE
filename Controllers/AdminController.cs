using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HUFLITCOFFEE.Models;
using HUFLITCOFFEE.Models.Main;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace HUFLITCOFFEE.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HuflitcoffeeContext _huflitcoffeeContext;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        // Constructor
        public AdminController(HuflitcoffeeContext huflitcoffeeContext, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _huflitcoffeeContext = huflitcoffeeContext;
            _httpContext = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AdminOrder()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("localDB")))
            {
                // Open the database connection
                await connection.OpenAsync();

                // SQL query to get all orders and their cart items
                string orderWithCartItemsSql = @"
        SELECT 
            o.OrderID, o.UserID, o.FullName, o.Address, o.PhoneNumber, o.Total, o.Status, o.DateOrder, o.Ghichu,
            ci.CartItemID, ci.ProductID, ci.ImgProduct, ci.NameProduct, ci.PriceProduct, ci.Quantity, ci.ToppingNames, ci.DVT, ci.UnitPrice
        FROM [Order] o
        LEFT JOIN CartItem ci ON o.UserID = ci.UserID";

                Dictionary<int, OrderDetailViewModel> orders = new Dictionary<int, OrderDetailViewModel>();

                // Execute the query
                using (SqlCommand command = new SqlCommand(orderWithCartItemsSql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int orderId = reader.GetInt32(0);

                            // If the order is not already in the dictionary, add it
                            if (!orders.ContainsKey(orderId))
                            {
                                orders[orderId] = new OrderDetailViewModel
                                {
                                    OrderID = orderId,
                                    UserId = reader.GetInt32(1),
                                    FullName = reader.GetString(2),
                                    Address = reader.GetString(3),
                                    PhoneNumber = reader.GetString(4),
                                    Total = reader.GetDecimal(5),
                                    Status = reader.GetString(6),
                                    DateOrder = reader.GetDateTime(7),
                                    Ghichu = reader.GetString(8),
                                    CartItems = new List<CartItem>()
                                };
                            }

                            // If there's a cart item, add it to the order's cart items list
                            if (!reader.IsDBNull(9)) // Check if CartItemID is not null
                            {
                                CartItem cartItem = new CartItem
                                {
                                    CartItemId = reader.GetInt32(9),
                                    UserId = reader.GetInt32(1),
                                    ProductId = reader.GetInt32(10),
                                    ImgProduct = reader.GetString(11),
                                    NameProduct = reader.GetString(12),
                                    PriceProduct = reader.GetDecimal(13),
                                    Quantity = reader.GetInt32(14),
                                    ToppingNames = reader.GetString(15),
                                    Dvt = reader.GetString(16),
                                    UnitPrice = reader.GetDecimal(17)
                                };
                                orders[orderId]?.CartItems?.Add(cartItem);
                            }
                        }
                    }
                }

                // Return the view with the list of orders
                return View(orders.Values.ToList());
            }
        }

        [HttpPost("/admin/updateorder")]
        public async Task<IActionResult> UpdateOrder(
  [FromForm] int orderid,
  [FromForm] string status)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("localDB")))
                {
                    await connection.OpenAsync();

                    string sql = @"
            UPDATE [Order]
            SET Status = @Status
            WHERE OrderID = @OrderID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Status", status);
                        command.Parameters.AddWithValue("@OrderID", orderid);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true, message = "đơn hàng đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = $"Lỗi khi cập nhật đơn hàng: {ex.Message}" });
            }
        }

        public IActionResult AdminDelivery()
        {
            return View();
        }

        public IActionResult AdminProduct()
        {
            var products = _huflitcoffeeContext.Products.ToList();
            return View(products);
        }

        // Action để hiển thị form thêm sản phẩm
        // public IActionResult AddProduct()
        // {
        //     return View();
        // }

        // Action xử lý khi người dùng nhấn nút Lưu trên form
        [HttpPost("/admin/addproduct")]
        public async Task<IActionResult> AddProduct(
             [FromForm] int IdCagory,
      [FromForm] string product_name,
      [FromForm] string product_price,
      [FromForm] string product_size,
      [FromForm] string product_category,
      [FromForm] string product_description,
      [FromForm] IFormFile product_image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string imageUrl = await SaveImageAsync(product_image);

                    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("localDB")))
                    {
                        await connection.OpenAsync();

                        string sql = @"
                    INSERT INTO Product ( NameProduct, PriceProduct, Dvt, DescriptionProduct, NameCategory, ImgProduct, CategoryID)
                    VALUES (@NameProduct, @PriceProduct, @Dvt, @DescriptionProduct, @NameCategory, @ImgProduct ,@CategoryID)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@NameProduct", product_name);
                            command.Parameters.AddWithValue("@PriceProduct", decimal.Parse(product_price));
                            command.Parameters.AddWithValue("@Dvt", product_size);
                            command.Parameters.AddWithValue("@DescriptionProduct", product_description);
                            command.Parameters.AddWithValue("@NameCategory", product_category);
                            command.Parameters.AddWithValue("@ImgProduct", imageUrl);
                            command.Parameters.AddWithValue("@CategoryID", IdCagory);
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return Json(new { success = true, message = "Sản phẩm đã được thêm thành công." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return Json(new { success = false, message = $"Lỗi khi lưu sản phẩm: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }


        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("File ảnh không hợp lệ.");
            }

            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/images/" + uniqueFileName;
        }


        // Xử lý edit sản phẩm

        [HttpPost("/admin/editproduct")]
        public async Task<IActionResult> EditProduct(
    [FromForm] int product_id,
    [FromForm] string product_name,
    [FromForm] string product_price,
    [FromForm] string product_size,
    [FromForm] string product_category,
    [FromForm] string product_description,
    [FromForm] IFormFile product_image,
    [FromForm] string product_image_url)
        {
            try
            {
                string imageUrl = product_image_url; // Mặc định là URL hiện tại

                if (product_image != null && product_image.Length > 0)
                {
                    // Nếu có file ảnh mới được chọn, lưu ảnh và lấy URL mới
                    imageUrl = await SaveImageAsync(product_image);
                }

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("localDB")))
                {
                    await connection.OpenAsync();

                    string sql = @"
            UPDATE Product
            SET NameProduct = @NameProduct,
                PriceProduct = @PriceProduct,
                Dvt = @Dvt,
                DescriptionProduct = @DescriptionProduct,
                NameCategory = @NameCategory,
                ImgProduct = @ImgProduct
            WHERE ProductId = @ProductId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@NameProduct", product_name);
                        command.Parameters.AddWithValue("@PriceProduct", decimal.Parse(product_price));
                        command.Parameters.AddWithValue("@Dvt", product_size);
                        command.Parameters.AddWithValue("@DescriptionProduct", product_description);
                        command.Parameters.AddWithValue("@NameCategory", product_category);
                        command.Parameters.AddWithValue("@ImgProduct", imageUrl);
                        command.Parameters.AddWithValue("@ProductId", product_id);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true, message = "Sản phẩm đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = $"Lỗi khi cập nhật sản phẩm: {ex.Message}" });
            }
        }
        // Xử lý delete sản phẩm
        // Action xử lý khi nhận yêu cầu POST từ form xóa sản phẩm
        [HttpPost("/admin/deleteproduct")]
        public async Task<IActionResult> DeleteProduct([FromForm] int delete_product_id)
        {
            try
            {
                // Thực hiện kết nối đến cơ sở dữ liệu
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("localDB")))
                {
                    await connection.OpenAsync();

                    // Chuẩn bị câu truy vấn SQL để xóa sản phẩm dựa vào ProductId
                    string sql = @"
                        DELETE FROM Product
                        WHERE ProductId = @ProductId
                    ";

                    // Sử dụng SqlCommand để thực thi câu truy vấn
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", delete_product_id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Json(new { success = true, message = "Xóa sản phẩm thành công." });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Không tìm thấy sản phẩm để xóa." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = $"Lỗi khi xóa sản phẩm: {ex.Message}" });
            }
        }
        public IActionResult AdminCustomer()
        {
            var users = _huflitcoffeeContext.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> ThongKeBaoCao()
        {
            // Lấy danh sách đơn hàng từ database và chuyển sang danh sách
            var orders = await _huflitcoffeeContext.Orders.ToListAsync();

            // Tính tổng thanh toán của tất cả đơn hàng
            var totalPayment = orders.Sum(c => c.Total);
            var totalOrders = orders.Count();

            if (orders.Count > 0)
            {
                // Lấy đơn hàng mới nhất và đơn hàng cũ nhất
                var latestOrderDate = orders.Max(o => o.DateOrder);
                var earliestOrderDate = orders.Min(o => o.DateOrder);

                // Tính tổng số ngày bán hàng
                var totalDaysSelling = (latestOrderDate - earliestOrderDate)?.Days;
                ViewBag.TotalDaysSelling = totalDaysSelling;
                ViewBag.EarliestOrderDate = earliestOrderDate?.ToString("dd-MM-yyyy");
                ViewBag.LatestOrderDate = latestOrderDate?.ToString("dd-MM-yyyy");
            }
            else
            {
                ViewBag.TotalDaysSelling = "Không có đơn hàng nào.";
            }
            // Truyền tổng thanh toán đến view thông qua ViewBag
            ViewBag.TotalPayment = (decimal)totalPayment;
            ViewBag.TotalOrders = totalOrders;
            // Trả về danh sách các đơn hàng để hiển thị trên view
            return View(orders);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
