using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Books.APP.Domain;

namespace Books.APP.Features.Books
{
    public class BookCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(200)]
        public string Name { get; set; }

        public short? NumberOfPages { get; set; }

        public DateTime PublishDate { get; set; }

        public decimal Price { get; set; }

        public bool IsTopSeller { get; set; }

        public int PublisherId { get; set; }

        public List<int> AuthorIds { get; set; } = new List<int>();

        public List<int> CategoryIds { get; set; } = new List<int>();
    }

    public class BookCreateHandler : Service<Book>, IRequestHandler<BookCreateRequest, CommandResponse>
    {
        public BookCreateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(BookCreateRequest request, CancellationToken cancellationToken)
        {
            if (await DbSet().AnyAsync(b => b.Name == request.Name.Trim() && b.PublisherId == request.PublisherId, cancellationToken))
                return Error("A book with the same name from this publisher already exists!");
            if (!await DbSet<Publisher>().AnyAsync(p => p.Id == request.PublisherId, cancellationToken))
                return Error("Publisher not found!");
            foreach (var authorId in request.AuthorIds.Distinct())
            {
                if (!await DbSet<Author>().AnyAsync(a => a.Id == authorId, cancellationToken))
                    return Error("One or more authors were not found!");
            }
            foreach (var categoryId in request.CategoryIds.Distinct())
            {
                if (!await DbSet<Category>().AnyAsync(c => c.Id == categoryId, cancellationToken))
                    return Error("One or more categories were not found!");
            }
            var entity = new Book
            {
                Name = request.Name.Trim(),
                NumberOfPages = request.NumberOfPages,
                PublishDate = request.PublishDate,
                Price = request.Price,
                IsTopSeller = request.IsTopSeller,
                PublisherId = request.PublisherId,
                AuthorIds = request.AuthorIds,
                CategoryIds = request.CategoryIds
            };
            await CreateAsync(entity, cancellationToken);
            return Success("Book created successfully.", entity.Id);
        }
    }
}
