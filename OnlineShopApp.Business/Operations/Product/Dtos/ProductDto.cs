using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopApp.Business.Operations.Product.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }  // Ürün ID'si
        public string ProductName { get; set; }  // Ürün adı
        public decimal Price { get; set; }  // Ürün fiyatı
        public int StockQuantity { get; set; }  // Stok miktarı
    }
}
