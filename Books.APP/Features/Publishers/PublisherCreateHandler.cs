using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Books.APP.Domain;

namespace Books.APP.Features.Publishers
{
    public class PublisherCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(150)]
        public string Name { get; set; }

        public int? FoundedYear { get; set; }

        public bool IsActive { get; set; }

        [StringLength(300)]
        public string Website { get; set; }
    }

    public class PublisherCreateHandler : Service<Publisher>, IRequestHandler<PublisherCreateRequest, CommandResponse>
    {
        public PublisherCreateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(PublisherCreateRequest request, CancellationToken cancellationToken)
        {
            if (await DbSet().AnyAsync(p => p.Name == request.Name.Trim(), cancellationToken))
                return Error("Publisher with the same name already exists!");
            var entity = new Publisher
            {
                Name = request.Name.Trim(),
                FoundedYear = request.FoundedYear,
                IsActive = request.IsActive,
                Website = request.Website?.Trim()
            };
            await CreateAsync(entity, cancellationToken);
            return Success("Publisher created successfully.", entity.Id);
        }
    }
}
