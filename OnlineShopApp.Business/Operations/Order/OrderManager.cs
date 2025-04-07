using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineShopApp.Business.Operations.Order;
using OnlineShopApp.Business.Operations.Order.Dtos;
using OnlineShopApp.Business.Types;
using OnlineShopApp.Data.Entity;
using OnlineShopApp.Data.Repositories;
using OnlineShopApp.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using OrderProductDtoBusiness = OnlineShopApp.Business.Operations.Order.Dtos.OrderProductDto;

public class OrderManager : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OrderEntity> _orderRepository;
    private readonly IRepository<OrderProductEntity> _orderProductRepository;
    private readonly IRepository<ProductEntity> _productRepository;

    public OrderManager(
        IUnitOfWork unitOfWork,
        IRepository<OrderEntity> orderRepository,
        IRepository<OrderProductEntity> orderProductRepository,
        IRepository<ProductEntity> productRepository
       )
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _orderProductRepository = orderProductRepository;
        _productRepository = productRepository;
        
    }

  
    public async Task<ServiceMessage<PagedResult<OrderDto>>> GetAllOrders(int page, int pageSize)
    {
        var query = _orderRepository
            .GetAll()
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product);

        var totalCount = await query.CountAsync();

        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (!orders.Any())
        {
            return new ServiceMessage<PagedResult<OrderDto>>()
            {
                IsSucceed = false,
                Message = "Sipariş bulunamadı.",
                Data = null
            };
        }

        var orderDtos = orders.Select(order => new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Products = order.OrderProducts?.Select(p => new OrderProductDto
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                Price = p.Product.Price,
                ProductName=p.Product.ProductName

                


            }).ToList()
        }).ToList();


        return new ServiceMessage<PagedResult<OrderDto>>()
        {
            IsSucceed = true,
            Message = "Orders retrieved successfully.",
            Data = new PagedResult<OrderDto>
            {
                Items = orderDtos,
                TotalCount = totalCount
            }
        };
    }



    public async Task<OrderDto> GetOrder(int orderId)
    {
        var orderEntity = await _orderRepository.GetAll(x => x.Id == orderId)
            .Include(x => x.OrderProducts)  
            .ThenInclude(op => op.Product) 
            .FirstOrDefaultAsync();

        if (orderEntity == null)
            return null; 

        var orderDto = new OrderDto
        {
            Id = orderEntity.Id,
            CustomerId = orderEntity.CustomerId,
            OrderDate = orderEntity.OrderDate,
            TotalAmount = orderEntity.TotalAmount,
            Products = orderEntity.OrderProducts.Select(op => new OrderProductDto
            {
                ProductId = op.ProductId,
                Quantity = op.Quantity,
                Price = op.Product.Price ,
                ProductName = op.Product.ProductName
            }).ToList()
        };

        return orderDto;
    }
    public async Task<ServiceMessage> AddOrder(AddOrderDto order)
    {
        try
        {
            
            if (order.Products.Count == 0 || order.Products.Exists(p => p.ProductId <= 0 || p.Quantity <= 0))
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz ürün ID'si veya miktar."
                };
            }

            await _unitOfWork.BeginTransaction(); 

            
            var orderEntity = new OrderEntity
            {
                CustomerId = order.CustomerId,
                OrderDate = DateTime.Now,
                TotalAmount = 0, 
                OrderProducts = new List<OrderProductEntity>()
            };

            _orderRepository.Add(orderEntity); 
            await _unitOfWork.SaveChangesAsync(); 

            decimal totalAmount = 0;
            
            foreach (var product in order.Products)
            {
                var productEntity =  _productRepository.GetById(product.ProductId); 
                if (productEntity == null)
                {
                    
                    await _unitOfWork.RollBackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = $"Ürün bulunamadı. ID: {product.ProductId}"
                    };
                }

                if (productEntity.StockQuantity < product.Quantity)
                {
                   
                    await _unitOfWork.RollBackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = $"Yeterli stok yok. ID: {product.ProductId} ürününden {product.Quantity} adet istenmiş."
                    };
                }

                
                totalAmount += productEntity.Price * product.Quantity;

                
                _orderProductRepository.Add(new OrderProductEntity
                {
                    OrderId = orderEntity.Id,
                    ProductId = product.ProductId,
                    Quantity = product.Quantity
                });

                // Stok miktarını güncelliyoruz
                productEntity.StockQuantity -= product.Quantity; 
                _productRepository.Update(productEntity); 
            }

            
            orderEntity.TotalAmount = totalAmount;

            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Sipariş başarıyla eklendi."
            };
        }
        catch (Exception ex)
        {
            
            await _unitOfWork.RollBackTransaction();
            return new ServiceMessage
            {
                IsSucceed = false,
                Message = "Sipariş kaydı sırasında bir hata oluştu: " + ex.Message 
            };
        }
    }

    public async Task<ServiceMessage> UpdateProductQuantity(int orderId, int productId, int newQuantity)
    {
        var orderEntity = await _orderRepository.GetAll(x => x.Id == orderId)
            .Include(x => x.OrderProducts)
            .FirstOrDefaultAsync();

        if (orderEntity == null)
        {
            return new ServiceMessage
            {
                IsSucceed = false,
                Message = "Sipariş bulunamadı."
            };
        }

        var orderProduct = orderEntity.OrderProducts.FirstOrDefault(op => op.ProductId == productId);

        if (orderProduct == null)
        {
            return new ServiceMessage
            {
                IsSucceed = false,
                Message = "Ürün siparişin içinde bulunamadı."
            };
        }

        
        orderProduct.Quantity = newQuantity;
        _orderProductRepository.Update(orderProduct);


        
        decimal totalAmount = 0;

        foreach (var product in orderEntity.OrderProducts)
        {
            var productEntity =  _productRepository.GetById(product.ProductId);
            if (productEntity != null)
            {
                totalAmount += productEntity.Price * product.Quantity;
            }
        }

        orderEntity.TotalAmount = totalAmount;
        _orderRepository.Update(orderEntity);
       

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollBackTransaction();
            return new ServiceMessage
            {
                IsSucceed = false,
                Message = $"Miktar güncellenirken bir hata oluştu: {ex.Message}"
            };
        }

        return new ServiceMessage
        {
            IsSucceed = true,
            Message = "Miktar başarıyla güncellendi ve toplam tutar yeniden hesaplandı."
        };
    }
    public async Task<ServiceMessage> DeleteOrder(int id)
    {
       
        var orderEntity = _orderRepository.GetById(id);
        if (orderEntity == null)
        {
            return new ServiceMessage
            {
                IsSucceed = false,
                Message = "Sipariş bulunamadı"
            };
        }

        
        var orderProducts =  _orderProductRepository.GetAll(x => x.OrderId == id);
        foreach (var orderProduct in orderProducts)
        {
           
            _orderProductRepository.Delete(orderProduct);
        }

        _orderRepository.Delete(orderEntity);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Sipariş ve ilgili ürünler başarıyla silindi."
            };
        }
        catch (Exception ex)
        {
            return new ServiceMessage
            {
                IsSucceed = false,
                Message = $"Bir hata oluştu: {ex.Message}"
            };
        }
    }
    public async Task<ServiceMessage> UpdateOrder(UpdateOrderDto updateOrderDto)
    {
        var order = _orderRepository.GetById(updateOrderDto.Id); 

        if (order == null)
        {
            return new ServiceMessage
            {
                IsSucceed = false,
                Message = "Sipariş bulunamadı." 
            };
        }

        order.CustomerId = updateOrderDto.CustomerId;

        decimal totalAmount = 0;

        for (int i = 0; i < updateOrderDto.ProductIds.Count; i++)
        {
            var product =  _productRepository.GetById(updateOrderDto.ProductIds[i]); 

            if (product == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"Ürün bulunamadı. ID: {updateOrderDto.ProductIds[i]}" 
                };
            }

            var orderProduct = order.OrderProducts.FirstOrDefault(op => op.ProductId == product.Id);
            if (orderProduct != null)
            {
               
                orderProduct.Quantity = updateOrderDto.Quantities[i];
                
                totalAmount += product.Price * updateOrderDto.Quantities[i];
            }
        }
        
        order.TotalAmount = totalAmount;
        _orderRepository.Update(order);

        

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Sipariş başarıyla güncellendi."
            };
        }
        catch (Exception ex)
        {
            return new ServiceMessage
            {
                IsSucceed = false,
                Message = "Güncellenirken bir hata oluştu: " + ex.Message
            };
        }
    }


}

