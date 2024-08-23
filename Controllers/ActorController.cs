using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETickets.Controllers
{
    public class ActorController : Controller
    {
        # region Services Handling
        private readonly IActorRepository _actorRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IActorMovieRepository _actorMovieRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITicketsCartRepository _ticketsCartRepository;

        public ActorController(IActorRepository actorRepository,
            IMovieRepository movieRepository,
            IActorMovieRepository actorMovieRepository,
            UserManager<ApplicationUser> userManager,
            ITicketsCartRepository ticketsCartRepository)
        {
            _actorRepository = actorRepository;
            _movieRepository = movieRepository;
            _actorMovieRepository = actorMovieRepository;
            _userManager = userManager;
            _ticketsCartRepository = ticketsCartRepository;
        }
        #endregion
        #region Show All Actors
        public IActionResult Index() {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _ticketsCartRepository.Get(e => e.UserId == userId && e.TicketsStatus == Data.TicketsStatus.Shopping, c => c.TCinema).ToList();
                ViewData["TotalTickets"] = result?.Sum(e => e.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
            }
            return View(_actorRepository.GetAll().ToList());
        }
        #endregion
        #region Show Specific Actor Details 
        public IActionResult Details(int id) {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _ticketsCartRepository.Get(e => e.UserId == userId && e.TicketsStatus == Data.TicketsStatus.Shopping, c => c.TCinema).ToList();
                ViewData["TotalTickets"] = result?.Sum(e => e.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
            }
            return View(_actorRepository.Get(actor => actor.ActorId == id, m => m.Movies).FirstOrDefault());
        }
        #endregion
        #region Add New Actor
        [Authorize(Roles = "Admin")]
        public IActionResult AddActor()
        {
            ViewData["Show"] = false;
            ViewData["Movies"] = _movieRepository?.GetAll().ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddActor(Actor actor, List<int> Selectedmovies)
        {
            if (ModelState.IsValid)
            {
                #region image
                actor.ProfilePicture = $"{Guid.NewGuid()}{Path.GetExtension(actor.ImageFile?.FileName)}";
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cast", actor.ProfilePicture);
                actor.ImageFile?.CopyTo(new FileStream(imagePath, FileMode.Create));
                #endregion
                #region New Actor
                _actorRepository?.CreateNew(actor);
                _actorRepository?.Commit();
                #endregion
                #region Add Movies to New Actor
                var act = _actorRepository?.Get(a => a == actor).FirstOrDefault();
                if (act != null)
                {
                    foreach (var movie in Selectedmovies)
                        _actorMovieRepository.CreateNew(new ActorMovie { ActorId = act.ActorId, MovieId = movie });
                    _actorMovieRepository.Commit();
                }
                #endregion
                #region Go to the Details of the Actor
                return RedirectToAction("Details", "Actor", new { id = act?.ActorId });
                #endregion
            }
            ViewData["Movies"] = _movieRepository?.GetAll().ToList();
            return View(actor);
        }
        #endregion
        #region Edit Actor
        [Authorize(Roles = "Admin")]
        public IActionResult EditActor(int id)
        {
            ViewData["Show"] = false;
            ViewData["Movies"] = _movieRepository?.GetAll().ToList();
            return View(_actorRepository?.Get(act => act.ActorId == id, movies => movies.Movies).FirstOrDefault());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditActor(Actor actor, List<int> SelectedMovies)
        {
            if (ModelState.IsValid)
            {
                #region image
                if (actor.ImageFile != null)
                {
                    actor.ProfilePicture = $"{Guid.NewGuid()}{Path.GetExtension(actor.ImageFile?.FileName)}";
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cast", actor.ProfilePicture);
                    actor.ImageFile?.CopyTo(new FileStream(imagePath, FileMode.Create));
                }
                #endregion
                #region Edit Actor
                _actorRepository?.Edit(actor);
                _actorRepository?.Commit();
                #endregion
                #region Edit Movies in Actor
                var act = _actorRepository?.Get(a => a.ActorId == actor.ActorId, mov => mov.Movies).FirstOrDefault();
                if (act != null)
                {
                    foreach (var movie in act.Movies)
                        if (SelectedMovies.Contains(movie.MovieId))
                            SelectedMovies.Remove(movie.MovieId);
                        else
                            _actorMovieRepository.Delete(_actorMovieRepository.Get(mv => (mv.MovieId == movie.MovieId && mv.ActorId == act.ActorId)).FirstOrDefault());
                    foreach (var movie in SelectedMovies)
                        _actorMovieRepository.CreateNew(new ActorMovie { ActorId = act.ActorId, MovieId = movie });
                    _actorMovieRepository?.Commit();
                }
                #endregion
                #region Go to the Details of the Actor
                return RedirectToAction("Details", "Actor", new { id = actor.ActorId });
                #endregion
            }

            ViewData["Movies"] = _movieRepository?.GetAll().ToList();

            return View(actor);
        }
        #endregion
        #region Delete Actor
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteActor(int id)
        {
            var actor = _actorRepository?.Get(act => act.ActorId == id, mv => mv.ActorMovies).FirstOrDefault();
            if (actor != null)
            {
                foreach (var movie in actor.ActorMovies)
                    _actorMovieRepository.Delete(movie);
                
                //Search For It Later
                //var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cast", actor.ProfilePicture);

                //if (System.IO.File.Exists(imagePath))                
                //    System.IO.File.Delete(imagePath);
                
                _actorRepository?.Delete(actor);
                _actorRepository?.Commit();
            }
            return RedirectToAction("Index", "Actor");
        }
        #endregion
    }
}
