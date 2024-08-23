using ETickets.Models;
using ETickets.Models.ViewModel;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ETickets.Controllers
{
    public class HomeController : Controller
    {
        #region Services Handling
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly IActorRepository _actorRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITicketsCartRepository _ticketsCartRepository;

        public HomeController(ILogger<HomeController> logger,
            IMovieRepository movieRepository,
            IActorRepository actorRepository, 
            ICinemaRepository cinemaRepository,
            UserManager<ApplicationUser> userManager,
            ITicketsCartRepository ticketsCartRepository)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _actorRepository = actorRepository;
            _cinemaRepository = cinemaRepository;
            _userManager = userManager;
            _ticketsCartRepository = ticketsCartRepository;
        }
        #endregion
        #region Home Page Shows All Movies
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _ticketsCartRepository.Get(e => e.UserId == userId && e.TicketsStatus == Data.TicketsStatus.Shopping, c => c.TCinema).ToList();
                ViewData["Tickets"] = result;
                ViewData["TotalTickets"] = result?.Sum(e => e.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
            }

            return View(_movieRepository.Get(e => e.MovieId > 0,
                                            cat => cat.Categories, Cinema => Cinema.Cinemas).ToList());

        }
        #endregion
        #region Search
        public IActionResult Search(string SearchFor,string SearchQuery)
        {
            if (string.IsNullOrWhiteSpace(SearchQuery) || SearchQuery.Length < 1) return RedirectToAction("Index");
           
            ViewData["SearchQuery"] = SearchQuery;
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _ticketsCartRepository.Get(e => e.UserId == userId && e.TicketsStatus == Data.TicketsStatus.Shopping, c => c.TCinema).ToList();
                ViewData["TotalTickets"] = result?.Sum(e => e.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
            }
            var SearchResult = new SearchView
            {
                Movies = SearchFor == "All" || SearchFor == "Movies" ?
			       _movieRepository.Get(mov => mov.Title.ToLower().Contains(SearchQuery.ToLower()),
                                            cat => cat.Categories, Cinema => Cinema.Cinemas).ToList() : null,

                Actors = SearchFor == "All" || SearchFor == "Actors" ?
			       _actorRepository.Get(act => act.Name.ToLower().Contains(SearchQuery.ToLower())).ToList() : null,

                Cinemas = SearchFor == "All" || SearchFor == "Cinemas" ?
			        _cinemaRepository.Get(cin => cin.Name.ToLower().Contains(SearchQuery.ToLower())).ToList() : null
			};
            return View(SearchResult);
        }
        #endregion
        #region Default
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}
