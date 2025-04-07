using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopApp.Business.Operations.Product.Dtos
{
    public class UpdateProductDto
    {
        public int Id { get; set; } // Ürünü güncellemek için ID gerekli
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

    }
}
