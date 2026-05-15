using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Books.APP.Domain;

namespace Books.APP.Features.Publishers
{
    public class PublisherUpdateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(150)]
        public string Name { get; set; }

        public int? FoundedYear { get; set; }

        public bool IsActive { get; set; }

        [StringLength(300)]
        public string Website { get; set; }
    }

    public class PublisherUpdateHandler : Service<Publisher>, IRequestHandler<PublisherUpdateRequest, CommandResponse>
    {
        public PublisherUpdateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(PublisherUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await DbSet().AnyAsync(p => p.Id != request.Id && p.Name == request.Name.Trim(), cancellationToken))
                return Error("Publisher with the same name already exists!");
            var entity = await DbSet().SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Publisher not found!");
            entity.Name = request.Name.Trim();
            entity.FoundedYear = request.FoundedYear;
            entity.IsActive = request.IsActive;
            entity.Website = request.Website?.Trim();
            await UpdateAsync(entity, cancellationToken);
            return Success("Publisher updated successfully.", entity.Id);
        }
    }
}
