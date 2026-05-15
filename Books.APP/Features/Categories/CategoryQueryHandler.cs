using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Categories
{
    public class CategoryQueryRequest : Request, IRequest<IQueryable<CategoryQueryResponse>>
    {
    }

    public class CategoryQueryResponse : Response
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsArchived { get; set; }
        public int BookCount { get; set; }
        public string IsArchivedF { get; set; }
    }

    public class CategoryQueryHandler : Service<Category>, IRequestHandler<CategoryQueryRequest, IQueryable<CategoryQueryResponse>>
    {
        public CategoryQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Category> DbSet()
        {
            return base.DbSet().Include(c => c.BookCategories).OrderBy(c => c.SortOrder).ThenBy(c => c.Name);
        }

        public Task<IQueryable<CategoryQueryResponse>> Handle(CategoryQueryRequest request, CancellationToken cancellationToken)
        {
            var query = DbSet().Select(c => new CategoryQueryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                SortOrder = c.SortOrder,
                IsArchived = c.IsArchived,
                BookCount = c.BookCategories.Count,
                IsArchivedF = c.IsArchived ? "Archived" : "Active"
            });
            return Task.FromResult(query);
        }
    }
}
