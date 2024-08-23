using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;

namespace ETickets.Controllers
{
    public class MovieController : Controller
    {
        #region Services Handling
        private readonly IMovieRepository _movieRepository;
        private readonly IActorMovieRepository _actorMovieRepository;
        private readonly ICinemaMovieRepository _cinemaMovieRepository;
        private readonly IActorRepository _actorRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITicketsCartRepository _ticketsCartRepository;

        public MovieController(IMovieRepository movieRepository,
            IActorMovieRepository actorMovieRepository,
            ICinemaMovieRepository cinemaMovieRepository,
            IActorRepository actorRepository,
            ICinemaRepository cinemaRepository,
            ICategoriesRepository categoriesRepository,
             UserManager<ApplicationUser> userManager,
            ITicketsCartRepository ticketsCartRepository)
        {
            _movieRepository = movieRepository;
            _actorMovieRepository = actorMovieRepository;
            _cinemaMovieRepository = cinemaMovieRepository;
            _actorRepository = actorRepository;
            _cinemaRepository = cinemaRepository;
            _categoriesRepository = categoriesRepository;
            _userManager = userManager;
            _ticketsCartRepository = ticketsCartRepository;
        }
        #endregion
        #region Movie Details
        public IActionResult Index(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _ticketsCartRepository.Get(e => e.UserId == userId && e.TicketsStatus == Data.TicketsStatus.Shopping, c => c.TCinema).ToList();
                ViewData["Tickets"] = result;
                ViewData["TotalTickets"] = result?.Sum(e => e.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
            }
            var movie = _movieRepository.
            Get(movie => movie.MovieId == id
            , cat => cat.Categories
            , actors => actors.Actors
            , Cinema => Cinema.Cinemas).FirstOrDefault();

            if(movie == null)
                return NotFound();
            movie.Views++;
            _movieRepository.Edit(movie);
            _movieRepository.Commit();
            return View(movie);
        }
        #endregion
        #region New Movie
        [Authorize(Roles = "Admin")]
        public IActionResult AddMovie()  
        {
            ViewData["Show"] = false;
            ViewData["Actors"] = _actorRepository.GetAll();
            ViewData["Cinemas"] = _cinemaRepository.GetAll();
            ViewData["Categories"] = _categoriesRepository.GetAll();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMovie(Movie movie,List<int> SelectedActors,List<int> SelectedCinemas)
        {
            if(ModelState.IsValid)
            {
                if(movie.StartDate < movie.EndDate)
                {
                    #region image
                    movie.ImgUrl = $"{Guid.NewGuid()}{Path.GetExtension(movie.ImageFile?.FileName)}";
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", movie.ImgUrl);
                    movie.ImageFile?.CopyTo(new FileStream(imagePath, FileMode.Create));
                    #endregion
                    #region New Movie
                    _movieRepository.CreateNew(movie);
                    _movieRepository.Commit();
                    #endregion
                    #region Add Actors and Cinemas to New Movie
                    var mov = _movieRepository.Get(m => m == movie).FirstOrDefault();
                    foreach(var actor in SelectedActors)
                        _actorMovieRepository.CreateNew(new ActorMovie { ActorId = actor,MovieId = mov.MovieId });
                    foreach (var cinema in SelectedCinemas)
                        _cinemaMovieRepository.CreateNew(new CinemaMovie { CinemaId = cinema, MovieId = mov.MovieId });
                    #endregion
                    #region Save all new Data To DB
                    _actorMovieRepository.Commit();
                    _cinemaMovieRepository.Commit();
                    #endregion
                    #region Go to the Details of the Movie
                    return RedirectToAction("Index", "Movie",new { id = mov?.MovieId });
                    #endregion
                }
                TempData["DateProblem"] = "End date must be after start date.";
            }
            ViewData["Actors"] = _actorRepository.GetAll();
            ViewData["Cinemas"] = _cinemaRepository.GetAll();
            ViewData["Categories"] = _categoriesRepository.GetAll();
            return View(movie);
        }
        #endregion
        #region Edit Movie
        [Authorize(Roles = "Admin")]
        public IActionResult EditMovie(int id)
        {
            ViewData["Show"] = false;
            ViewData["Actors"] = _actorRepository.GetAll();
            ViewData["Cinemas"] = _cinemaRepository.GetAll();
            ViewData["Categories"] = _categoriesRepository.GetAll().Select(e => new SelectListItem(e.Name, e.CategoriesId.ToString()));
            return View(_movieRepository.Get(mov => mov.MovieId == id,actor => actor.Actors,Cinema => Cinema.Cinemas).FirstOrDefault());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMovie(Movie movie, List<int> SelectedActors, List<int> SelectedCinemas)
        { 
            if (ModelState.IsValid)
            {
                if (movie.StartDate < movie.EndDate)
                {
                    #region image
                    if(movie.ImageFile != null) 
                    {
                        movie.ImgUrl = $"{Guid.NewGuid()}{Path.GetExtension(movie.ImageFile?.FileName)}";
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", movie.ImgUrl);
                        movie.ImageFile?.CopyTo(new FileStream(imagePath, FileMode.Create));
                    }
                    #endregion
                    #region Edit Movie
                    _movieRepository.Edit(movie);
                    _movieRepository.Commit();
                    #endregion
                    var mov = _movieRepository.Get(m => m.MovieId == movie.MovieId, act => act.Actors, Cin => Cin.Cinemas).FirstOrDefault();
                    #region Edit Cast in Movie
                    foreach (var actor in mov.Actors)
                        if (SelectedActors.Contains(actor.ActorId))
                            SelectedActors.Remove(actor.ActorId);
                        else
                            _actorMovieRepository.Delete(_actorMovieRepository.Get(ac => (ac.MovieId == movie.MovieId && ac.ActorId == actor.ActorId)).FirstOrDefault());
                    foreach (var actor in SelectedActors)
                        _actorMovieRepository.CreateNew(new ActorMovie { ActorId = actor, MovieId = movie.MovieId });
                    _actorMovieRepository.Commit();
                    #endregion
                    #region Edit Movie in which Cinemas 
                    foreach (var cinema in mov.Cinemas)
                        if (SelectedCinemas.Contains(cinema.CinemaId))
                            SelectedCinemas.Remove(cinema.CinemaId);
                        else
                            _cinemaMovieRepository.Delete(_cinemaMovieRepository.Get(ci => (ci.MovieId == movie.MovieId && ci.CinemaId == cinema.CinemaId)).FirstOrDefault());
                    foreach (var cinema in SelectedCinemas)
                        _cinemaMovieRepository.CreateNew(new CinemaMovie { CinemaId = cinema, MovieId = movie.MovieId });
                    _cinemaMovieRepository.Commit();
                    #endregion
                    #region Go to the Details of the Movie
                    return RedirectToAction("Index", "Movie", new { id = movie.MovieId });
                    #endregion
                }
                TempData["DateProblem"] = "End date must be after start date.";
            }
            ViewData["Actors"] = _actorRepository.GetAll();
            ViewData["Cinemas"] = _cinemaRepository.GetAll();
            ViewData["Categories"] = _categoriesRepository.GetAll().Select(e => new SelectListItem(e.Name, e.CategoriesId.ToString()));
            var EditMovieData = _movieRepository.Get(m => m.MovieId == movie.MovieId).FirstOrDefault();
            foreach(var actor in SelectedActors)
                EditMovieData?.Actors.Add(_actorRepository?.Get(a => a.ActorId == actor).FirstOrDefault() ?? new Actor());
            foreach (var cinema in SelectedCinemas)
                EditMovieData?.Cinemas.Add(_cinemaRepository.Get(c => c.CinemaId == cinema).FirstOrDefault()!);

            return View(EditMovieData);
        }
        #endregion
        #region Delete Movie
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteMovie(int id)
        {
            var movie = _movieRepository.Get(mov => mov.MovieId ==  id,ac => ac.ActorMovies,Ci => Ci.CinemaMovies).FirstOrDefault();
            
            if (movie == null) return NotFound();

            foreach(var actor in movie.ActorMovies)
                _actorMovieRepository.Delete(actor);
            foreach(var cinema in movie.CinemaMovies)
                _cinemaMovieRepository.Delete(cinema);
            _movieRepository.Delete(movie);
            _movieRepository.Commit();
            
            return RedirectToAction("Index","Home");
        }
        #endregion
    }
}
