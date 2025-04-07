namespace OnlineShopApp.WebApi.Models
{
    public class UpdateProductRequest
    {

        public string ProductName { get; set; }  
        public decimal Price { get; set; }  
        public int StockQuantity { get; set; } 
    }
}

