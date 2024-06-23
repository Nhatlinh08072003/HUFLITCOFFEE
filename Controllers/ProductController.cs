using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HUFLITCOFFEE.Models;
using HUFLITCOFFEE.Models.Main;
using System.Data.SqlClient;
using System.Security.Claims;

namespace HUFLITCOFFEE.Controllers;

public class ProductController : Controller
{
    private readonly HuflitcoffeeContext _huflitcoffeeContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IConfiguration _configuration;

    // Constructor
    public ProductController(HuflitcoffeeContext huflitcoffeeContext, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _huflitcoffeeContext = huflitcoffeeContext;
        _httpContext = httpContextAccessor;
        _configuration = configuration;
    }
    public IActionResult Index()
    {
        var products = _huflitcoffeeContext.Products.ToList();
        return View(products);
    }
    // Method to display product details
    [Route("detail/{id}")]
    public IActionResult Detail(int id)
    {
        var mainProduct = _huflitcoffeeContext.Products.FirstOrDefault(p => p.ProductId == id);
        if (mainProduct == null)
        {
            return NotFound();
        }

        // Lấy thêm các sản phẩm có ProductId là 73, 74 và 75
        var relatedProducts = _huflitcoffeeContext.Products
                                    .Where(p => p.ProductId == 73 || p.ProductId == 74 || p.ProductId == 75)
                                    .ToList();

        // Tạo một model chứa sản phẩm chính và các sản phẩm liên quan
        var model = new DetailViewModel
        {
            MainProduct = mainProduct,
            RelatedProducts = relatedProducts
        };

        return View(model); // Trả về view với danh sách sản phẩm
    }
    public IActionResult Cart()
    {
        var carts = _huflitcoffeeContext.CartItems.ToList();
        return View(carts);
    }
    public IActionResult AddToCart()
    {
        return View();
    }

    // Action xử lý khi người dùng nhấn nút thêm sản phẩm vào giỏ hàng trên form
    [HttpPost("/product/addtocart")]
    public async Task<IActionResult> AddToCart(
         [FromForm] int product_id,
  [FromForm] string product_name,
  [FromForm] string product_price,
  [FromForm] string product_image,
  [FromForm] string product_size,
  [FromForm] string product_quantity,
  [FromForm] string topping_names)
    {
        if (ModelState.IsValid)
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

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("CoffeeDBConnectionString")))
                {
                    await connection.OpenAsync();

                    string sql = @"
                    INSERT INTO CartItem  ( UserID, ProductID, ImgProduct,NameProduct,PriceProduct,Quantity,ToppingNames,DVT )
                    VALUES ( @UserID, @ProductID, @ImgProduct,@NameProduct,@PriceProduct,@Quantity,@ToppingNames,@DVT )";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@NameProduct", product_name);
                        command.Parameters.AddWithValue("@PriceProduct", decimal.Parse(product_price));
                        command.Parameters.AddWithValue("@ImgProduct", product_image);
                        command.Parameters.AddWithValue("@ProductID", product_id);
                        command.Parameters.AddWithValue("@Quantity", product_quantity);
                        command.Parameters.AddWithValue("@ToppingNames", topping_names);
                        command.Parameters.AddWithValue("@DVT", product_size);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = $"Lỗi khi lưu sản phẩm: {ex.Message}" });
            }
        }

        return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
    }
    // Xử lý delete sản phẩm khỏi giỏ hàng
    public IActionResult DeleteCart()
    {
        return View();
    }
    // Action xử lý khi nhận yêu cầu POST từ form xóa sản phẩm
    [HttpPost("/product/deletecart")]
    public async Task<IActionResult> DeleteCart([FromForm] int delete_cart_id)
    {
        try
        {
            // Thực hiện kết nối đến cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("CoffeeDBConnectionString")))
            {
                await connection.OpenAsync();

                // Chuẩn bị câu truy vấn SQL để xóa sản phẩm dựa vào ProductId
                string sql = @"
                        DELETE FROM CartItem
                        WHERE CartItemID = @CartItemID
                    ";

                // Sử dụng SqlCommand để thực thi câu truy vấn
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CartItemID", delete_cart_id);
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        return Json(new { success = true, message = "Xóa sản phẩm khỏi giỏ hàng thành công." });
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

    // Action xử lý khi nhận yêu cầu POST từ form xóa sản phẩm
    [HttpPost("/product/deleteall")]
    public async Task<IActionResult> DeleteAll()
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
            // Thực hiện kết nối đến cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("CoffeeDBConnectionString")))
            {
                await connection.OpenAsync();

                // Chuẩn bị câu truy vấn SQL để xóa sản phẩm dựa vào ProductId
                string sql = @"
                        DELETE FROM CartItem
                        WHERE UserID = @UserID
                    ";

                // Sử dụng SqlCommand để thực thi câu truy vấn
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        return Json(new { success = true, message = "Xóa tất cả sản phẩm khỏi giỏ hàng thành công." });
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



    public IActionResult Products()
    {
        return View();
    }
    public IActionResult Sale()
    {
        return View();
    }
    public IActionResult Shipping()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
