using Microsoft.EntityFrameworkCore;
using MyCeima.ViewModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCeima.Models
{

    [PrimaryKey(nameof(MoviesId), nameof(ApplicationUserId))]

    public class FinalCart
    {
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }


        public string MoviesId { get; set; }
        public Movies Movies { get; set; }

        public int Count { get; set; }


    }
}
