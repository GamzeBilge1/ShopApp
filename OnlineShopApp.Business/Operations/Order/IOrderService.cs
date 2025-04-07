using OnlineShopApp.Business.Operations.Order.Dtos;
using OnlineShopApp.Business.Operations.Product.Dtos;
using OnlineShopApp.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopApp.Business.Operations.Order
{
    public interface IOrderService
    {
        Task<ServiceMessage> AddOrder(AddOrderDto orderDto);
        Task<OrderDto> GetOrder(int orderId);
        Task<ServiceMessage> UpdateProductQuantity(int orderId, int productId, int newQuantity);
        Task<ServiceMessage> DeleteOrder(int orderId);
        Task<ServiceMessage> UpdateOrder(UpdateOrderDto orderDto);
        Task<ServiceMessage<PagedResult<OrderDto>>> GetAllOrders(int page, int pageSize);

       

       

    }
}
