using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Books.APP.Domain;

namespace Books.APP.Features.Categories
{
    public class CategoryCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public bool IsArchived { get; set; }
    }

    public class CategoryCreateHandler : Service<Category>, IRequestHandler<CategoryCreateRequest, CommandResponse>
    {
        public CategoryCreateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(CategoryCreateRequest request, CancellationToken cancellationToken)
        {
            if (await DbSet().AnyAsync(c => c.Name == request.Name.Trim(), cancellationToken))
                return Error("Category with the same name already exists!");
            var entity = new Category
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                SortOrder = request.SortOrder,
                IsArchived = request.IsArchived
            };
            await CreateAsync(entity, cancellationToken);
            return Success("Category created successfully.", entity.Id);
        }
    }
}
