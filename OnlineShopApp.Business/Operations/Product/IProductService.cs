using OnlineShopApp.Business.Operations.Product.Dtos;
using OnlineShopApp.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShopApp.Business.Operations.Product.Dtos;

namespace OnlineShopApp.Business.Operations.Product
{
    public interface IProductService
    {

        Task<ServiceMessage> AddProduct(AddProductDto product);

        Task<ProductDto> GetProduct(int id);
        Task<ServiceMessage> UpdateProduct(UpdateProductDto productDto);

        Task<ServiceMessage> DeleteProduct(int id);

        Task<ServiceMessage> UpdateStock(int id, int stockQuantity);

        Task<PagedResult<ProductDto>> GetAllProducts(int page, int pageSize);

    }
}
