using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopApp.WebApi.Models;
using System.Security.Claims;
using OnlineShopApp.Business.Operations.Order;
using OnlineShopApp.Business.Operations.Order.Dtos;
using OnlineShopApp.WebApi.Filters;
using OnlineShopApp.Business.Operations.User;


namespace OnlineShopApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }
    
        [HttpGet("paged")]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> GetAllOrders(int page = 1)
        {
            int pageSize = 3; 

            var result = await _orderService.GetAllOrders(page, pageSize);

            if (!result.IsSucceed)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }



        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrder(id);

            if (order == null)
            {
                return NotFound(new { Message = "Sipariş bulunamadı." });
            }

            return Ok(order);
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpPost]
        public async Task<IActionResult> AddOrder(AddOrderRequest request)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }
           
            var dto = new AddOrderDto
            {
                CustomerId = int.Parse(customerId), 
                Products = request.Products.Select(p => new Business.Operations.Order.Dtos.OrderProductDto
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList()
            };

            var result = await _orderService.AddOrder(dto);

            if (result.IsSucceed)
                return Ok("Sipariş başarıyla eklendi.");
            else
                return BadRequest(result.Message);
        }



        [Authorize(Roles = "Admin")]
        [HttpPatch("{orderId}/products/{productId}")]
        public async Task<IActionResult> UpdateProductQuantity(int orderId, int productId, [FromBody] UpdateProductQuantityRequest request)
        {
            
            var result = await _orderService.UpdateProductQuantity(orderId, productId, request.Quantity);

            if (!result.IsSucceed)
            {
                return BadRequest(result.Message);
            }

            return Ok("Miktar başarıyla güncellendi.");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);

            if (!result.IsSucceed)
                return NotFound(result.Message);

            return Ok(result.Message);
        }

    
        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        [TimeControlFilter]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderRequest request)
        {
            
            var existingOrder = await _orderService.GetOrder(id);

            if (existingOrder == null)
            {
                return NotFound("Sipariş bulunamadı.");
            }

            var updateOrderDto = new UpdateOrderDto
            {
                Id = id,
                CustomerId = request.CustomerId,
                ProductIds = request.ProductIds,
                Quantities = request.Quantities,
               
            };

            var result = await _orderService.UpdateOrder(updateOrderDto);

            if (!result.IsSucceed)
            {
                return BadRequest(result.Message);
            }

           
            return await GetOrder(id);
        }

    }
}
