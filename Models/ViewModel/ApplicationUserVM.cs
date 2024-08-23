using ETickets.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ETickets.Models.ViewModel
{
    public class ApplicationUserVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[A-Z][a-z]{2,}$", ErrorMessage = "First Name must start with a capital letter and be at least 3 characters long.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[A-Z][a-z]{2,}$", ErrorMessage = "Last Name must start with a capital letter and be at least 3 characters long.")]
        public string LastName { get; set; } = null!;
        [ValidateNever]
        public string? username { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format")]
        public DateTime DateBirth { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid Password Format")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirmation Password is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and Confirmation Password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(@"^(010|011|012|015)[0-9]{8}$", ErrorMessage = "Invalid Phone Number format.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        public bool RememberMe { get; set; }
    }

}
