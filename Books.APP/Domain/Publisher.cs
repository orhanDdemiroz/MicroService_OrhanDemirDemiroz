using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Books.APP.Domain
{
    public class Publisher : Entity
    {
        [Required, StringLength(150)]
        public string Name { get; set; }

        public int? FoundedYear { get; set; }

        public bool IsActive { get; set; }

        [StringLength(300)]
        public string Website { get; set; }

        public List<Book> Books { get; set; } = new List<Book>();
    }
}
