using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Mono.TextTemplating;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ETickets.Models
{
    public class Actor
    {
        #region Properities
        public int ActorId { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [ValidateNever]
        public string ProfilePicture { get; set; } = null!;
        [NotMapped]
        [ValidateNever]
        public IFormFile? ImageFile { get; set; }
        [Required]
        public string Bio { get; set; } = null !;
        [Required]
        public string Nationality { get; set; } = null!;
        [Required]
        public string Birth_Date { get; set; } = null!;
        #endregion
        #region Relations
        [ValidateNever]
        public List<ActorMovie> ActorMovies { get; set; } = [];
        [ValidateNever]
        public List<Movie> Movies { get; set; } = [];
        #endregion

        public override bool Equals(object? obj) =>
                      obj is Actor actor &&
                      this.Name == actor.Name &&
                      this.ProfilePicture == actor.ProfilePicture &&
                      this.Nationality == actor.Nationality &&
                      this.Bio == actor.Bio &&
                      this.Birth_Date == actor.Birth_Date;
        public override int GetHashCode() =>
        HashCode.Combine(Name, ProfilePicture, Nationality, Bio, Birth_Date);
    }
}
