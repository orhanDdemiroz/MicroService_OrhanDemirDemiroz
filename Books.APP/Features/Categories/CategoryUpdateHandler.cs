using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Books.APP.Domain;

namespace Books.APP.Features.Categories
{
    public class CategoryUpdateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public bool IsArchived { get; set; }
    }

    public class CategoryUpdateHandler : Service<Category>, IRequestHandler<CategoryUpdateRequest, CommandResponse>
    {
        public CategoryUpdateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(CategoryUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await DbSet().AnyAsync(c => c.Id != request.Id && c.Name == request.Name.Trim(), cancellationToken))
                return Error("Category with the same name already exists!");
            var entity = await DbSet().SingleOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Category not found!");
            entity.Name = request.Name.Trim();
            entity.Description = request.Description?.Trim();
            entity.SortOrder = request.SortOrder;
            entity.IsArchived = request.IsArchived;
            await UpdateAsync(entity, cancellationToken);
            return Success("Category updated successfully.", entity.Id);
        }
    }
}
