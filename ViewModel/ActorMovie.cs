using System.ComponentModel.DataAnnotations.Schema;

namespace MyCeima.ViewModel
{
    public class ActorMovie
    {
        public int Id { get; set; }
        public int ActorId { get; set; }
        public Actor Actor { get; set; }

      
        public int MoviesId { get; set; }
        public Movies Movie { get; set; }

      
    }
}
