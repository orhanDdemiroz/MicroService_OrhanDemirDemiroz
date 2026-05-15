using CORE.APP.Domain;

namespace Books.APP.Domain
{
    public class BookCategory : Entity
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
