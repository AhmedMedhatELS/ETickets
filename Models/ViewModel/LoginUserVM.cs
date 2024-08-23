using ETickets.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ETickets.Models.ViewModel
{
    public class LoginUserVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid Password Format")]
        public string Password { get; set; } = null!;
        
        public bool RememberMe { get; set; }

    }
}
