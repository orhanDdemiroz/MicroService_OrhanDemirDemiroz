using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Categories
{
    public class CategoryDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class CategoryDeleteHandler : Service<Category>, IRequestHandler<CategoryDeleteRequest, CommandResponse>
    {
        public CategoryDeleteHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Category> DbSet()
        {
            return base.DbSet().Include(c => c.BookCategories);
        }

        public async Task<CommandResponse> Handle(CategoryDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await DbSet().SingleOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Category not found!");
            if (entity.BookCategories.Any())
                return Error("Category can't be deleted because there are related books!");
            await DeleteAsync(entity, cancellationToken);
            return Success("Category deleted successfully.", entity.Id);
        }
    }
}
