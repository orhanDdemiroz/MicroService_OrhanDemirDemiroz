using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.APP.Domain
{
    public class Book : Entity
    {
        [Required, StringLength(200)]
        public string Name { get; set; }

        public short? NumberOfPages { get; set; }

        public DateTime PublishDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool IsTopSeller { get; set; }

        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        public List<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        public List<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

        [NotMapped]
        public List<int> AuthorIds
        {
            get => BookAuthors.Select(ba => ba.AuthorId).ToList();
            set => BookAuthors = value?.Select(aId => new BookAuthor { AuthorId = aId }).ToList()
                ?? new List<BookAuthor>();
        }

        [NotMapped]
        public List<int> CategoryIds
        {
            get => BookCategories.Select(bc => bc.CategoryId).ToList();
            set => BookCategories = value?.Select(cId => new BookCategory { CategoryId = cId }).ToList()
                ?? new List<BookCategory>();
        }
    }
}
