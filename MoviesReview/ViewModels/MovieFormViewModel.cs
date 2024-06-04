using MoviesReview.Models;
using System.ComponentModel.DataAnnotations;

namespace MoviesReview.ViewModels
{
    public class MovieFormViewModel
    {
        public int Id { get; set; }
        [Required , StringLength(250)]
        public string Title { get; set; }
        public int Year { get; set; }
        [Range(1,10)]
        public double Rate { get; set; }
        [Required, StringLength(2500)]
        public string StoreLine { get; set; }
        [Display(Name = "Select Poster")]
        public byte[] Poster { get; set; }
        [Display(Name ="Genre")]

        public byte GenreId { get; set; }

        public IEnumerable<Genre> Genres { get; set; }
        
    }
}
