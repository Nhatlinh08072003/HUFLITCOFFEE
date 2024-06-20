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
        public IActionResult AddProduct()
        {
            return View();
        }

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
