using Microsoft.EntityFrameworkCore;
using MyCeima.ViewModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCeima.Models
{
    [PrimaryKey(nameof(ApplicationUserId), nameof(MoviesId))]
    public class MovieCart
    {

            public string ApplicationUserId { get; set; }
            public ApplicationUser ApplicationUser { get; set; }

            public int MoviesId { get; set; }

            [ForeignKey(nameof(MoviesId))]

              public Movies Movies { get; set; }
            
            public int Count { get; set; }
        
    }










}
