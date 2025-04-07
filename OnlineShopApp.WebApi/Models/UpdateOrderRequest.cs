namespace OnlineShopApp.WebApi.Models
{
    public class UpdateOrderRequest
    {
        public int CustomerId { get; set; } 
        public List<int> ProductIds { get; set; } 
        public List<int> Quantities { get; set; } 
        
    }
}
