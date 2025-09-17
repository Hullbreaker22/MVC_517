using MyCeima.Models;

namespace MyCeima.ViewModel
{
    public class CartRequest
    {
        public string ApplicationUserId { get; set; }

        public int MoviesId { get; set; }

        public int Count { get; set; }

    }
}
