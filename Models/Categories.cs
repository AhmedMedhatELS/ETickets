using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace ETickets.Models
{
    public class Categories
    {
        #region Properities
        public int CategoriesId { get; set; }
        [Required]
		[RegularExpression(@"^[A-Z][a-zA-Z0-9\s-]*$", ErrorMessage = "Category name must start with an uppercase letter and can contain letters, numbers, spaces, and hyphens.")]
		public string Name { get; set; } = null!;
        #endregion
        #region Relations
        [ValidateNever]
        public ICollection<Movie> Posts { get; set; } = new List<Movie>();
        #endregion
    }
}
