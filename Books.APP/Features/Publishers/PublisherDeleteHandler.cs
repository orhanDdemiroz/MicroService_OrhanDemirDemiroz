using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Publishers
{
    public class PublisherDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class PublisherDeleteHandler : Service<Publisher>, IRequestHandler<PublisherDeleteRequest, CommandResponse>
    {
        public PublisherDeleteHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Publisher> DbSet()
        {
            return base.DbSet().Include(p => p.Books);
        }

        public async Task<CommandResponse> Handle(PublisherDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await DbSet().SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Publisher not found!");
            if (entity.Books.Any())
                return Error("Publisher can't be deleted because there are related books!");
            await DeleteAsync(entity, cancellationToken);
            return Success("Publisher deleted successfully.", entity.Id);
        }
    }
}
