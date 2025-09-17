using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class User
    {

        public int? UserId { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }
        
        public IFormFile? Photo { get; set; }

        public string? PhotoPath { get; set; }
        public DateTime? Created { get; set; }= DateTime.Now;
  
        public DateTime? Modified { get; set; }=DateTime.Now;
    }

    public class UserLoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }

    
}
