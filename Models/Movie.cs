using ETickets.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ETickets.Models
{
    public class Movie
    {
        #region Properities
        public int MovieId { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null !;
        [ValidateNever]
        public string ImgUrl { get; set; } = null!;
        [NotMapped]
        [ValidateNever]
        public IFormFile? ImageFile { get; set; }
        [Required]
        public string TrailerUrl { get; set; } = null!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [ValidateNever]
        public MovieStatus MovieStatus { 
            get {
                var currentDate = DateTime.Now;
                if (currentDate < StartDate)
                    return MovieStatus.Upcoming;
                else if (currentDate > EndDate)
                    return MovieStatus.Expired;
                else
                    return MovieStatus.Available;
            }
        }
        [ValidateNever]
        public int Views {  get; set; }
        #endregion
        #region Relations
        [Required]
        public int CategoriesId { get; set; }
        [ValidateNever]
        public Categories Categories { get; set; } = null!;
        [ValidateNever]
        public List<ActorMovie> ActorMovies { get; set; } = [];
        [ValidateNever]
        public List<CinemaMovie> CinemaMovies { get; set; } = [];
        [ValidateNever]
        public List<Actor> Actors { get; set; } = [];
        [ValidateNever]
        public List<Cinema> Cinemas { get; set; } = [];
        #endregion
        public override bool Equals(object? obj) =>
                       obj is Movie movie &&
                       this.Title == movie.Title &&
                       this.Price == movie.Price &&
                       this.ImgUrl == movie.ImgUrl &&
                       this.StartDate == movie.StartDate &&
                       this.EndDate == movie.EndDate;
        public override int GetHashCode() =>
        HashCode.Combine(Title, Price, ImgUrl, StartDate, EndDate);   
    }
}