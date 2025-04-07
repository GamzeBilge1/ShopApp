using OnlineShopApp.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnlineShopApp.Business.Operations.Order.Dtos
{

    public class AddOrderDto
    {
        
        public int CustomerId { get; set; }
        public List<OrderProductDto> Products { get; set; }
    }

   

}



