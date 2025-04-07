using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShopApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopApp.Data.Entity
{
    public class UserEntity : BaseEntity
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }


        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        
        public ICollection<OrderEntity> Orders { get; set; }
    }

    public class UserConfiguration : BaseConfiguration<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.Property(x => x.FirstName)
               .IsRequired()
               .HasMaxLength(50);

            builder.Property(x => x.LastName)
               .IsRequired()
               .HasMaxLength(50);

            base.Configure(builder);
        }
    }


}
