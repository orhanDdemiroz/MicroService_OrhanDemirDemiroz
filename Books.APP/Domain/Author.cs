using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Books.APP.Domain
{
    public class Author : Entity
    {
        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        public List<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
