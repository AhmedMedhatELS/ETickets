using ETickets.Data;
using Microsoft.AspNetCore.Identity;

namespace ETickets.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateofBirth { get; set; }
        public Gender Gender { get; set; }

    }
}
