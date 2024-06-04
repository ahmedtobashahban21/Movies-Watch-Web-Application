using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesReview.Models;
using MoviesReview.ViewModels;
using NToastNotify;

namespace MoviesReview.Controllers
{
    public class MoviesController : Controller
    {
        // first we need an instant from ApplicationDBContext
        private readonly ApplicationDBContext _context;
        private readonly IToastNotification _toastNotification;

        public MoviesController(ApplicationDBContext Context , IToastNotification toastNotification)
        {
            _context = Context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var Movies = await _context.Movies.OrderByDescending(x => x.Rate).ToListAsync();
            return View(Movies);
        }
        public async Task<IActionResult> Create()
        {
            var ViewModel = new MovieFormViewModel
            {
                Genres = await _context.Genres.OrderBy(m=>m.Name).ToListAsync()
            };
            return View("MovieForm", ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {

            //// we have a problem in ModelState
            //if (!ModelState.IsValid)
            //{
            //    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            //    return View(model);
            //}

            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please select a movie poster.");
                return View("MovieForm", model);
            }

            var poster = files.FirstOrDefault();
            if (poster == null)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please select a movie poster.");
                return View("MovieForm", model);
            }

            var allowedExtensions = new List<string> { ".jpg", ".png" };
            var fileExtension = Path.GetExtension(poster.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only PNG and JPG files are allowed!");
                return View("MovieForm", model);
            }

            if(poster.Length > 1048576) 
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.TryAddModelError("Poster", "Max size is 1 Miga");
                return View("MovieForm", model);
            }

            using var dataStream = new MemoryStream();  
            await poster.CopyToAsync(dataStream);
            var movie = new Movie
            {
                Title = model.Title,
                GenreId= model.GenreId,
                Year = model.Year,
                Rate = model.Rate,  
                StoreLine = model.StoreLine,    
                Poster  = dataStream.ToArray()
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();


            _toastNotification.AddSuccessToastMessage("Movie Created Successfuly");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            var viewModel = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                GenreId = movie.GenreId,
                Year = movie.Year,
                Rate = movie.Rate,
                StoreLine = movie.StoreLine,
                Poster = movie.Poster,
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            return View("MovieForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            //    return View(model);
            //}
            var movie = await _context.Movies.FindAsync(model.Id);
            if (movie == null)
                return NotFound();

            var files = Request.Form.Files;
            if (files.Any())
            {
                var poster = files.FirstOrDefault();
                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();

                var allowedExtensions = new List<string> { ".jpg", ".png" };
                var fileExtension = Path.GetExtension(poster.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only PNG and JPG files are allowed!");
                    return View("MovieForm", model);
                }

                if (poster.Length > 1048576)
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.TryAddModelError("Poster", "Max size is 1 Miga");
                    return View("MovieForm", model);
                }
                movie.Poster = model.Poster; 

            }

            movie.Title = model.Title;
            movie.GenreId = model.GenreId;
            movie.Year = model.Year;
            movie.Rate = model.Rate;
            movie.StoreLine = model.StoreLine;

            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Movie Updated Successfuly");

            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();
            var movie = await _context.Movies.Include(m=>m.Genre).SingleOrDefaultAsync(m=>m.Id == id);
            if(movie == null)
                return NotFound();
            return View(movie);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return Ok();
        }
    }
}
