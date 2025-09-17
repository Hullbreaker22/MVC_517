using MyCeima.Models;

namespace MyCeima.ViewModel
{
    public class UserOTP
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string OTPnumber { get; set; }
        public DateTime ValidTO{ get; set; }
        public ApplicationUser ApplicationUsers { get; set; }
    }
}
