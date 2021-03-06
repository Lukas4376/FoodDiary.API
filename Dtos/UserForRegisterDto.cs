using System.ComponentModel.DataAnnotations;

namespace FoodDiary.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(24, MinimumLength = 8, ErrorMessage = "You must specified password between 8 and 24")]
        public string Password { get; set; }
    }
}