using Microsoft.IdentityModel.Tokens;
using OnlineShopApp.Business.Operations.Product.Dtos;
using OnlineShopApp.Business.Types;
using OnlineShopApp.Data.Entity;
using OnlineShopApp.Data.Repositories;
using OnlineShopApp.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopApp.Business.Operations.Product
{
    public class ProductManager : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ProductEntity> _repository;

        public ProductManager(IUnitOfWork unitOfWork, IRepository<ProductEntity> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

       
        public async Task<ServiceMessage> AddProduct(AddProductDto product)
        {

            var hasProduct = _repository.GetAll(x => x.ProductName.ToLower() == product.ProductName.ToLower()).Any();

            if (hasProduct)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu isimde bir ürün zaten mevcut. Lütfen başka bir isim deneyin."
                };
            }
            var productEntity = new ProductEntity
            {
                ProductName = product.ProductName,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };

            _repository.Add(productEntity);

            //try
            //{
            //    await _unitOfWork.SaveChangesAsync();
            //}
            //catch (Exception)
            //{

            //    throw new Exception("Ürün kaydı sırasında bir hata oluştu.");

            //}

            await _unitOfWork.SaveChangesAsync();

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Ürün başarıyla eklendi."
            };


        }

        public async Task<ProductDto> GetProduct(int id)
        {
            var product = _repository.GetById(id);
            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };
        }

        public async Task<ServiceMessage> UpdateProduct(UpdateProductDto request)
        {
            var product = _repository.GetById(request.Id);
            if (product == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Ürün bulunamadı."
                };
            }

            product.ProductName = request.ProductName;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            _repository.Update(product);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Ürün güncellenirken bir hata oluştu.");
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Ürün başarıyla güncellendi."
            };
        }

        public async Task<ServiceMessage> UpdateStock(int id, int stockQuantity)
        {
            var product = _repository.GetById(id);
            if (product == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Ürün bulunamadı."
                };
            }

            product.StockQuantity = stockQuantity;
            _repository.Update(product);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Stok güncellenirken bir hata oluştu."
                };
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Stok başarıyla güncellendi."
            };
        }

        public async Task<ServiceMessage> DeleteProduct(int id)
        {
            var product = _repository.GetById(id);
            if (product == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Ürün bulunamadı."
                };
            }

            _repository.Delete(product);
            

            try
            {
                await _unitOfWork.SaveChangesAsync();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Ürün başarıyla silindi."
                };
            }
            catch (Exception ex)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Ürün silinirken hata oluştu: " + ex.Message
                };
            }
        }

    

        public async Task<PagedResult<ProductDto>> GetAllProducts(int page, int pageSize)
        {
            var query = _repository.GetAll();

            var totalCount = query.Count();

            var pagedProducts = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = pagedProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            });

            return new PagedResult<ProductDto>
            {
                Items = dtoList,
                TotalCount = totalCount
            };
        }

    }
}


