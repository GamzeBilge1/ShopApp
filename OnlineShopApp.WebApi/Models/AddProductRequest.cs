using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.WebApi.Models
{
    public class AddProductRequest
    {
        [Required(ErrorMessage = "Ürün adı boş bırakılamaz.")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Ürün adı 5 ile 30 karakter arasında olmalıdır.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, 100000, ErrorMessage = "Fiyat 0.01 ile 100000 arasında olmalıdır.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok miktarı zorunludur.")]
        [Range(0, 10000, ErrorMessage = "Stok miktarı 0 - 10000 arası bir değer olmalıdır.")]
        public int StockQuantity { get; set; }

    }
}
