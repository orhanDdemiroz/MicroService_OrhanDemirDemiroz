using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Publishers
{
    public class PublisherQueryRequest : Request, IRequest<IQueryable<PublisherQueryResponse>>
    {
    }

    public class PublisherQueryResponse : Response
    {
        public string Name { get; set; }
        public int? FoundedYear { get; set; }
        public bool IsActive { get; set; }
        public string Website { get; set; }
        public int BookCount { get; set; }
        public string IsActiveF { get; set; }
    }

    public class PublisherQueryHandler : Service<Publisher>, IRequestHandler<PublisherQueryRequest, IQueryable<PublisherQueryResponse>>
    {
        public PublisherQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Publisher> DbSet()
        {
            return base.DbSet().Include(p => p.Books).OrderBy(p => p.Name);
        }

        public Task<IQueryable<PublisherQueryResponse>> Handle(PublisherQueryRequest request, CancellationToken cancellationToken)
        {
            var query = DbSet().Select(p => new PublisherQueryResponse
            {
                Id = p.Id,
                Name = p.Name,
                FoundedYear = p.FoundedYear,
                IsActive = p.IsActive,
                Website = p.Website,
                BookCount = p.Books.Count,
                IsActiveF = p.IsActive ? "Active" : "Inactive"
            });
            return Task.FromResult(query);
        }
    }
}
