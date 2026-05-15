using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Authors
{
    public class AuthorQueryRequest : Request, IRequest<IQueryable<AuthorQueryResponse>>
    {
    }

    public class AuthorQueryResponse : Response
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int BookCount { get; set; }
    }

    public class AuthorQueryHandler : Service<Author>, IRequestHandler<AuthorQueryRequest, IQueryable<AuthorQueryResponse>>
    {
        public AuthorQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Author> DbSet()
        {
            return base.DbSet().Include(a => a.BookAuthors).OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public Task<IQueryable<AuthorQueryResponse>> Handle(AuthorQueryRequest request, CancellationToken cancellationToken)
        {
            var query = DbSet().Select(a => new AuthorQueryResponse
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                FullName = a.FirstName + " " + a.LastName,
                BookCount = a.BookAuthors.Count
            });
            return Task.FromResult(query);
        }
    }
}
