using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;
using Books.APP.Features.Authors;
using Books.APP.Features.Categories;
using Books.APP.Features.Publishers;

namespace Books.APP.Features.Books
{
    public class BookQueryRequest : Request, IRequest<IQueryable<BookQueryResponse>>
    {
    }

    public class BookQueryResponse : Response
    {
        public string Name { get; set; }
        public short? NumberOfPages { get; set; }
        public DateTime PublishDate { get; set; }
        public decimal Price { get; set; }
        public bool IsTopSeller { get; set; }
        public int PublisherId { get; set; }
        public List<int> AuthorIds { get; set; }
        public List<int> CategoryIds { get; set; }

        public string PublishDateF { get; set; }
        public string PriceF { get; set; }
        public string IsTopSellerF { get; set; }
        public string NumberOfPagesF { get; set; }
        public string PublisherF { get; set; }
        public List<string> AuthorsF { get; set; }
        public List<string> CategoriesF { get; set; }

        public PublisherQueryResponse Publisher { get; set; }
        public List<AuthorQueryResponse> Authors { get; set; }
        public List<CategoryQueryResponse> Categories { get; set; }
    }

    public class BookQueryHandler : Service<Book>, IRequestHandler<BookQueryRequest, IQueryable<BookQueryResponse>>
    {
        public BookQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Book> DbSet()
        {
            return base.DbSet()
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
                .OrderByDescending(b => b.IsTopSeller).ThenBy(b => b.Name);
        }

        public Task<IQueryable<BookQueryResponse>> Handle(BookQueryRequest request, CancellationToken cancellationToken)
        {
            var query = DbSet().Select(b => new BookQueryResponse
            {
                Id = b.Id,
                Name = b.Name,
                NumberOfPages = b.NumberOfPages,
                PublishDate = b.PublishDate,
                Price = b.Price,
                IsTopSeller = b.IsTopSeller,
                PublisherId = b.PublisherId,
                AuthorIds = b.AuthorIds,
                CategoryIds = b.CategoryIds,

                PublishDateF = b.PublishDate.ToString("MM/dd/yyyy"),
                PriceF = b.Price.ToString("C2"),
                IsTopSellerF = b.IsTopSeller ? "Top Seller" : "Regular",
                NumberOfPagesF = b.NumberOfPages.HasValue ? b.NumberOfPages.Value + " pages" : string.Empty,
                PublisherF = b.Publisher.Name,

                Publisher = new PublisherQueryResponse
                {
                    Id = b.Publisher.Id,
                    Name = b.Publisher.Name,
                    FoundedYear = b.Publisher.FoundedYear,
                    IsActive = b.Publisher.IsActive,
                    Website = b.Publisher.Website
                },

                AuthorsF = b.BookAuthors.Select(ba => ba.Author.FirstName + " " + ba.Author.LastName).ToList(),
                Authors = b.BookAuthors.Select(ba => new AuthorQueryResponse
                {
                    Id = ba.Author.Id,
                    FirstName = ba.Author.FirstName,
                    LastName = ba.Author.LastName,
                    FullName = ba.Author.FirstName + " " + ba.Author.LastName
                }).ToList(),

                CategoriesF = b.BookCategories.Select(bc => bc.Category.Name).ToList(),
                Categories = b.BookCategories.Select(bc => new CategoryQueryResponse
                {
                    Id = bc.Category.Id,
                    Name = bc.Category.Name,
                    Description = bc.Category.Description,
                    SortOrder = bc.Category.SortOrder,
                    IsArchived = bc.Category.IsArchived
                }).ToList()
            });
            return Task.FromResult(query);
        }
    }
}
