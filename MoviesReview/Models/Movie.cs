using System.ComponentModel.DataAnnotations;

namespace MoviesReview.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required , MaxLength(250)]
        public string Title { get; set; }
        public int Year {  get; set; }  
        public double Rate {  get; set; }
        [Required , MaxLength(2500)]
        public string StoreLine { get; set; }
        [Required]
        public byte[] Poster {  get; set; }
        // create relation 
        public byte GenreId {  get; set; }
        public Genre Genre { get; set; } 
    }
}
