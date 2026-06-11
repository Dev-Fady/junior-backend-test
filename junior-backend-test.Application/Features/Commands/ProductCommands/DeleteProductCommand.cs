using junior_backend_test.Application.Wapper;
using unior_backend_test.Domain.Interfaces;
using junior_backend_test.Domain.Model;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace junior_backend_test.Application.Features.Commands.ProductCommands
{
    public class DeleteProductCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }

        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Response<string>>
    {
        private readonly IGenericRepository<Product> _productRepository;

        public DeleteProductCommandHandler(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Response<string>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIDAsync(request.Id);
            if (product == null)
            {
                return Response<string>.Fail("Product not found");
            }

            _productRepository.Archived(product);
            await _productRepository.SaveChangesAsync();

            return Response<string>.Success(null, "Product deleted successfully");
        }
    }
}
