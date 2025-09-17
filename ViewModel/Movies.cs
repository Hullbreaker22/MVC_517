using MyCeima.Models;
using System.ComponentModel.DataAnnotations;

namespace MyCeima.ViewModel
{
    public class Movies
    {

        public int Id { get; set; }
        [Required]
        public string? Name { get; set; } = string.Empty;
        [Required]

        public string? Description { get; set; } = string.Empty;
        [Required]

        public double Price { get; set; }
        [Required]

        public string ImgUrl { get; set; }
        public string? TrailerUrl { get; set; } = string.Empty;
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 
        public MovieStatus MovieStatus { get; set; }
        [Required]


        public int CinemaId { get; set; }
        [Required]

        public int CategoryId { get; set; }

        public Cinema Cinema { get; set; }
        public Category Category { get; set; }

        //public List<Actor> Actors { get; set; }

        public List<ActorMovie> ActorMovie { get; set; }


    }
}
