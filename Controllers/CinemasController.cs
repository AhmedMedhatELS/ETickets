using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETickets.Controllers
{
    public class CinemasController : Controller
    {
        # region Services Handling
        private readonly ICinemaRepository? _cinemaRepository;
        private readonly IMovieRepository? _movieRepository;
        private readonly ICinemaMovieRepository _cinemaMovieRepository;
        private readonly ITicketsCartRepository _ticketsCartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public CinemasController(ICinemaRepository? cinemaRepository,
            IMovieRepository movieRepository, 
            ICinemaMovieRepository cinemaMovieRepository,
            ITicketsCartRepository ticketsCartRepository,
            UserManager<ApplicationUser> userManager)
        {
            _cinemaRepository = cinemaRepository;
            _movieRepository = movieRepository;
            _cinemaMovieRepository = cinemaMovieRepository;
            _ticketsCartRepository = ticketsCartRepository;
            _userManager = userManager;
        }
        #endregion
        #region Show All Cinemas
        public IActionResult Index() {
            var userId = _userManager.GetUserId(User);
            if(userId != null)
            {
                var result = _ticketsCartRepository.Get(e => e.UserId == userId && e.TicketsStatus == Data.TicketsStatus.Shopping, c => c.TCinema).ToList();
                ViewData["TotalTickets"] = result?.Sum(e => e.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
            }
            return View(_cinemaRepository?.GetAll().ToList());
        }
        #endregion
        #region Show Specific Cinema Details 
        public IActionResult Details(int id) 
            {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _ticketsCartRepository.Get(e => e.UserId == userId && e.TicketsStatus == Data.TicketsStatus.Shopping, c => c.TCinema).ToList();
                ViewData["TotalTickets"] = result?.Sum(e => e.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
            }
            return View(_cinemaRepository?.Get(cin => cin.CinemaId == id, m => m.Movies).FirstOrDefault());
        }
        #endregion
        #region Add New Cinema
        [Authorize(Roles = "Admin")]
        public IActionResult AddCinema()
        {
            ViewData["Show"] = false;
            ViewData["Movies"] = _movieRepository?.GetAll().ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCinema(Cinema cinema, List<int> Selectedmovies)
        {
            if (ModelState.IsValid)
            {
                #region image
                cinema.img = $"{Guid.NewGuid()}{Path.GetExtension(cinema.ImageFile?.FileName)}";
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinema", cinema.img);
                cinema.ImageFile?.CopyTo(new FileStream(imagePath, FileMode.Create));
                #endregion
                #region New Cinema
                _cinemaRepository?.CreateNew(cinema);
                _cinemaRepository?.Commit();
                #endregion
                #region Add Movies to New Cinema
                var cin = _cinemaRepository?.Get(c => c == cinema).FirstOrDefault();
                if (cin != null)
                {
                    foreach (var movie in Selectedmovies)
                        _cinemaMovieRepository.CreateNew(new CinemaMovie { CinemaId = cin.CinemaId, MovieId = movie });
                    _cinemaMovieRepository.Commit();
                }
                #endregion
                #region Go to the Details of the Cinema
                return RedirectToAction("Details", "Cinemas", new { id = cin?.CinemaId });
                #endregion
            }
            ViewData["Movies"] = _movieRepository?.GetAll().ToList();
            return View(cinema);
        }
        #endregion
        #region Edit Cinema
        [Authorize(Roles = "Admin")]
        public IActionResult EditCinema(int id)
        {
            ViewData["Show"] = false;
            ViewData["Movies"] = _movieRepository?.GetAll().ToList();
            return View(_cinemaRepository?.Get(cin => cin.CinemaId == id, movies => movies.Movies).FirstOrDefault());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCinema(Cinema cinema, List<int> SelectedMovies)
        {
            if (ModelState.IsValid)
            {
                #region image
                if (cinema.ImageFile != null)
                {
                    cinema.img = $"{Guid.NewGuid()}{Path.GetExtension(cinema.ImageFile?.FileName)}";
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", cinema.img);
                    cinema.ImageFile?.CopyTo(new FileStream(imagePath, FileMode.Create));
                }
                #endregion
                #region Edit Cinema
                _cinemaRepository?.Edit(cinema);
                _cinemaRepository?.Commit();
                #endregion
                #region Edit Movies in Cinema
                var cin = _cinemaRepository?.Get(c => c.CinemaId == cinema.CinemaId, mov => mov.Movies).FirstOrDefault();
                if (cin != null)
                {
                    foreach (var movie in cin.Movies)
                        if (SelectedMovies.Contains(movie.MovieId))
                            SelectedMovies.Remove(movie.MovieId);
                        else
                            _cinemaMovieRepository?.Delete(_cinemaMovieRepository?.Get(mv => (mv.MovieId == movie.MovieId && mv.CinemaId == cin.CinemaId)).FirstOrDefault() ?? new CinemaMovie());
                    foreach (var movie in SelectedMovies)
                        _cinemaMovieRepository?.CreateNew(new CinemaMovie { CinemaId = cin.CinemaId, MovieId = movie });
                    _cinemaMovieRepository?.Commit();
                }
                #endregion
                #region Go to the Details of the Cinema
                return RedirectToAction("Details", "Cinemas", new { id = cinema.CinemaId });
                #endregion
            }

            ViewData["Movies"] = _movieRepository?.GetAll().ToList();
           
            return View(cinema);
        }
        #endregion
        #region Delete Cinema
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCinema(int id)
        {
            var cinema = _cinemaRepository?.Get(cin => cin.CinemaId == id, mv => mv.CinemaMovies).FirstOrDefault();
            if (cinema != null)
            {
                foreach (var movie in cinema.CinemaMovies)
                    _cinemaMovieRepository.Delete(movie);
                _cinemaRepository?.Delete(cinema);
                _cinemaRepository?.Commit();
            }
            return RedirectToAction("Index", "Cinemas");
        }
        #endregion

    }
}
