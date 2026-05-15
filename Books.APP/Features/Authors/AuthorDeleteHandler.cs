using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Authors
{
    public class AuthorDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class AuthorDeleteHandler : Service<Author>, IRequestHandler<AuthorDeleteRequest, CommandResponse>
    {
        public AuthorDeleteHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Author> DbSet()
        {
            return base.DbSet().Include(a => a.BookAuthors);
        }

        public async Task<CommandResponse> Handle(AuthorDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await DbSet().SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Author not found!");
            if (entity.BookAuthors.Any())
                return Error("Author can't be deleted because the author is linked to one or more books!");
            await DeleteAsync(entity, cancellationToken);
            return Success("Author deleted successfully.", entity.Id);
        }
    }
}
