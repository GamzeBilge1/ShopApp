using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using OnlineShopApp.Business.DataProtection;
using OnlineShopApp.Business.Operations.User.Dtos;
using OnlineShopApp.Business.Types;
using OnlineShopApp.Data.Entity;
using OnlineShopApp.Data.Enums;
using OnlineShopApp.Data.Repositories;
using OnlineShopApp.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopApp.Business.Operations.User
{
    public class UserManager : IUserService
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IDataProtection _protector;
        private readonly IMemoryCache _cache;

        public UserManager(IUnitOfWork unitofWork, IRepository<UserEntity> userRepository, IDataProtection protector, IMemoryCache cache)
        {
            _unitofWork = unitofWork;
            _userRepository = userRepository;
            _protector = protector;
            _cache = cache;

        }


        public async Task<ServiceMessage> AddUser(AddUserDto user)
        {
            var hasMail = _userRepository.GetAll(x => x.Email.ToLower() == user.Email.ToLower());
            if (hasMail.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Email adresi zaten sisteme kayıtlı"

                };
            }

            var hasPhone = _userRepository.GetAll(x => x.PhoneNumber == user.PhoneNumber);
            if (hasPhone.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu telefon numarası zaten sistemde kayıtlı."
                };
            }

            var userEntity = new UserEntity() 
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = _protector.Protect(user.Password), 
                PhoneNumber = user.PhoneNumber,
                Role = UserRole.Customer

            };

            _userRepository.Add(userEntity);

            //try
            //{
            //    await _unitofWork.SaveChangesAsync();
            //}
            //catch (Exception)
            //{
            //    throw new Exception("Kullanıcı kaydı sırasında bir hata ile karşılaşıldı");
            //}

            await _unitofWork.SaveChangesAsync();

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

       
        public ServiceMessage<UserInfoDto> LoginUser(LoginUserDto user)
        {

            
            // Caching

            if (_cache.TryGetValue(user.Email, out UserInfoDto cachedUser))
            {
                
                return new ServiceMessage<UserInfoDto>
                { 
                    
                    IsSucceed = true,
                    Data = cachedUser
                };
            }

            
            var userEntity = _userRepository.Get(x => x.Email.ToLower() == user.Email.ToLower()); 

            if (userEntity is null)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "Mail veya şifre hatalı "
                };
            }

            var unprotectedPassword = _protector.UnProtect(userEntity.Password); 

            if (unprotectedPassword == user.Password)
            {
                var userInfo = new UserInfoDto
                {
                    Id = userEntity.Id,
                    Email = userEntity.Email,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    Role = userEntity.Role,
                };

              

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30)); 


                _cache.Set(user.Email, userInfo, cacheEntryOptions);

                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = true,
                    Data = userInfo
                };
            }
            else
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "Mail veya şifre hatalı "
                };
            }

        }
    }
}
