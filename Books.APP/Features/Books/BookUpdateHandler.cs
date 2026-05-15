using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Books.APP.Domain;

namespace Books.APP.Features.Books
{
    public class BookUpdateRequest : Request, IRequest<CommandResponse>
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

    public class BookUpdateHandler : Service<Book>, IRequestHandler<BookUpdateRequest, CommandResponse>
    {
        public BookUpdateHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Book> DbSet()
        {
            return base.DbSet().Include(b => b.BookAuthors).Include(b => b.BookCategories);
        }

        public async Task<CommandResponse> Handle(BookUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await DbSet().AnyAsync(b => b.Id != request.Id && b.Name == request.Name.Trim() && b.PublisherId == request.PublisherId, cancellationToken))
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
            var entity = await DbSet().SingleOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Book not found!");
            Delete(entity.BookAuthors);
            Delete(entity.BookCategories);
            entity.Name = request.Name.Trim();
            entity.NumberOfPages = request.NumberOfPages;
            entity.PublishDate = request.PublishDate;
            entity.Price = request.Price;
            entity.IsTopSeller = request.IsTopSeller;
            entity.PublisherId = request.PublisherId;
            entity.AuthorIds = request.AuthorIds;
            entity.CategoryIds = request.CategoryIds;
            await UpdateAsync(entity, cancellationToken);
            return Success("Book updated successfully.", entity.Id);
        }
    }
}
