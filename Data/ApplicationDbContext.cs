using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ETickets.Models;
using ETickets.Models.ViewModel;

namespace ETickets.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Models
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Cinema> Cinema { get; set; }
        public DbSet<ActorMovie> ActorMovie { get; set; }
        public DbSet<CinemaMovie> cinemaMovies { get; set; }
        public DbSet<TicketsCart> TicketsCart { get; set; }
        public DbSet<TicketsCinema> TicketsCinemas { get; set; }
        #endregion

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }
        public ApplicationDbContext()
        {

        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);

        //    var Builder = new ConfigurationBuilder().
        //        AddJsonFile("appsettings.json", true,reloadOnChange: true).
        //        Build().
        //        GetConnectionString("DefaultConnection");

        //    optionsBuilder.UseSqlServer(Builder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>(eb => {
                eb.HasMany(e => e.Actors).WithMany(e => e.Movies).UsingEntity<ActorMovie>();
                eb.HasMany(e => e.Cinemas).WithMany(e => e.Movies).UsingEntity<CinemaMovie>();
                eb.Ignore(e => e.MovieStatus);
            });
        }
        
    }
}
