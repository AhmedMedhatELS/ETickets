using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETickets.Models
{
    public class Cinema
    {
        #region Properities
        public int CinemaId { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [ValidateNever]
        public string img { get; set; } = null!;
        [NotMapped]
        [ValidateNever]
        public IFormFile? ImageFile { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(@"^(010|011|012|015)[0-9]{8}$", ErrorMessage = "Invalid Phone Number format.")]
        public string phone { get; set; } = null!;
        [Required]
        public string city { get; set; } = null!;
        [Required]
        public string State { get; set; } = null!;
        [Required]
        public string Address { get; set; } = null!;
        #endregion
        #region Relations
        [ValidateNever]
        public List<CinemaMovie> CinemaMovies { get; set; } = [];
        [ValidateNever]
        public List<Movie> Movies { get; set; } = [];
        #endregion

        public override bool Equals(object? obj) =>
                      obj is Cinema cinema &&
                      this.Name == cinema.Name &&
                      this.img == cinema.img &&
                      this.phone == cinema.phone &&
                      this.city == cinema.city &&
                      this.State == cinema.State&&
                      this.Address == cinema.Address;
        public override int GetHashCode() =>
        HashCode.Combine(Name, img, phone, city, State,Address);
    }
}
