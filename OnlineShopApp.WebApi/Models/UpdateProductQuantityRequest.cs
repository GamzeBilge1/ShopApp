using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.WebApi.Models
{
    public class UpdateProductQuantityRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Stok miktarı en az 1 olmalıdır.")]
        public int Quantity { get; set; } 
    }
}
