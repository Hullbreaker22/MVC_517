using MyCeima.Models;

namespace MyCeima.ViewModel
{
    public class Promotions
    {

        public int Id  { get; set; }
        public string? Code { get; set; }
        public bool? Status { get; set; }
        public int Usage { get; set; }
        public DateTime ValidTo { get; set; }
        public string?  ApplicationUserId { get; set; }
        public ApplicationUser Applicationuser { get; set; }
    }
}
