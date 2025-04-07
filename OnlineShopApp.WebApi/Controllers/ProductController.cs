using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopApp.Business.Operations.Product;
using OnlineShopApp.Business.Operations.Product.Dtos;
using OnlineShopApp.WebApi.Models;

namespace OnlineShopApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddProduct(AddProductRequest request)
        {

            var addProductDto = new AddProductDto
            {
                ProductName = request.ProductName,
                StockQuantity=request.StockQuantity,
                Price= request.Price

            };

            var result = await _productService.AddProduct(addProductDto);

            if (result.IsSucceed)
                return Ok();
            else
                return BadRequest(result.Message);

        }



        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1)
        {
            int pageSize = 2; 
            var result = await _productService.GetAllProducts(page, pageSize);
            return Ok(result);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var result = await _productService.GetProduct(id);

            if (result == null)
                return NotFound(new { Message = "Ürün bulunamadı." });

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request)
        {
           
            var dto = new UpdateProductDto
            {
                Id = id,
                ProductName = request.ProductName,
                Price = request.Price,
                StockQuantity = request.StockQuantity
            };

            var result = await _productService.UpdateProduct(dto);

            if (result.IsSucceed)
                return Ok(new { Message = "Ürün başarıyla güncellendi." });
            else
                return BadRequest(result.Message);
        }

        [HttpPatch("{id}/stock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStock(int id, StockUpdateRequest request)
        {
            var result = await _productService.UpdateStock(id, request.StockQuantity);

            if (result.IsSucceed)
                return Ok(new { Message = "Stok başarıyla güncellendi." });

            return BadRequest(result.Message);
        }

      
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);

            if (result.IsSucceed)
                return Ok(new { Message = "Ürün başarıyla silindi." });
            else
                return NotFound(new { Message = "Ürün bulunamadı." });
        }
    }

}

