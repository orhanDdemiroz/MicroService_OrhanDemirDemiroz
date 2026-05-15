using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Books
{
    public class BookDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class BookDeleteHandler : Service<Book>, IRequestHandler<BookDeleteRequest, CommandResponse>
    {
        public BookDeleteHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Book> DbSet()
        {
            return base.DbSet().Include(b => b.BookAuthors).Include(b => b.BookCategories);
        }

        public async Task<CommandResponse> Handle(BookDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await DbSet().SingleOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Book not found!");
            Delete(entity.BookAuthors);
            Delete(entity.BookCategories);
            await DeleteAsync(entity, cancellationToken);
            return Success("Book deleted successfully.", entity.Id);
        }
    }
}
