using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.WebApi.Models
{
    public class RegisterRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Telefon numarası 10 haneli olmalı ve yalnızca rakamlardan oluşmalıdır.(Bitişik yazınız)")]
        public string PhoneNumber { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
