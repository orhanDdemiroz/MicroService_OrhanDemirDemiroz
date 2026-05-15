using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Books.APP.Domain
{
    public class Category : Entity
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public bool IsArchived { get; set; }

        public List<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}
