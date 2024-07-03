using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HUFLITCOFFEE.Models;
using HUFLITCOFFEE.Models.Main;
using System.Data.SqlClient;

namespace HUFLITCOFFEE.Controllers
{
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

        public IActionResult AdminOrder()
        {
            return View();
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

                    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("CoffeeDBConnectionString")))
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
        public IActionResult EditProduct()
        {
            return View();
        }

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

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("CoffeeDBConnectionString")))
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
        public IActionResult DeleteProduct()
        {
            return View();
        }
        // Action xử lý khi nhận yêu cầu POST từ form xóa sản phẩm
        [HttpPost("/admin/deleteproduct")]
        public async Task<IActionResult> DeleteProduct([FromForm] int delete_product_id)
        {
            try
            {
                // Thực hiện kết nối đến cơ sở dữ liệu
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("CoffeeDBConnectionString")))
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
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
