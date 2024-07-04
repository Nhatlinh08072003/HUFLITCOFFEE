document.addEventListener("DOMContentLoaded", function () {
   var iconMenu = document.getElementById("Home_icon_menu");
   var menuShowBehind = document.querySelector(".Home_menu_show_behind");
   var menuShow = document.querySelector(".Home_menu_show");
   var iconClose = document.getElementById("Home_icon_close");
   iconMenu.addEventListener("click", function () {
      menuShowBehind.style.display = "block";
   });
   iconClose.addEventListener("click", function () {
      menuShowBehind.style.display = "none";
   });
   document.addEventListener("click", function (event) {
      // Kiểm tra nếu không phải là menu hoặc icon menu thì ẩn menu
      if (
         !menuShow.contains(event.target) &&
         menuShowBehind.contains(event.target) &&
         event.target !== iconMenu
      ) {
         menuShowBehind.style.display = "none";
      }
   });
});
document.addEventListener("DOMContentLoaded", function () {
   var productsOption = document.getElementById("Home_products");
   var menuProducts = document.querySelector(".Home_menu_products");
   var isMenuVisible = false; // Biến để theo dõi trạng thái của menu

   // Khi hover vào Home_products
   productsOption.addEventListener("mouseover", function () {
      menuProducts.style.display = "block";
      setTimeout(function () {
         menuProducts.style.transform = "translateY(-45%)"; // Hiển thị từ dưới lên trên
      }, 10); // Thêm setTimeout để tránh xung đột với display: block
      isMenuVisible = true; // Đặt trạng thái là hiển thị
   });

   // Khi rời khỏi Home_products
   productsOption.addEventListener("mouseout", function () {
      if (!isMenuVisible) {
         // Kiểm tra nếu menu không hiển thị ở nơi khác
         menuProducts.style.display = "none";
      }
   });

   // Khi hover vào Home_menu_products
   menuProducts.addEventListener("mouseover", function () {
      isMenuVisible = true; // Đặt trạng thái là hiển thị
   });

   // Khi rời khỏi Home_menu_products
   menuProducts.addEventListener("mouseout", function () {
      isMenuVisible = false; // Đặt trạng thái là ẩn
   });

   // Khi chuột rời khỏi cửa sổ trình duyệt
   document.body.addEventListener("mouseout", function (event) {
      if (!menuProducts.contains(event.relatedTarget) && event.target !== productsOption) {
         // Kiểm tra nếu chuột không nằm trong menu và không phải vào Home_products
         menuProducts.style.display = "none"; // Ẩn menu
         menuProducts.style.transform = "translateY(50%)";
         isMenuVisible = false; // Đặt trạng thái là ẩn
      }
   });
});
