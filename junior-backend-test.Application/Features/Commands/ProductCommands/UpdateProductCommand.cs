using junior_backend_test.Application.Wapper;
using unior_backend_test.Domain.Interfaces;
using junior_backend_test.Domain.Model;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace junior_backend_test.Application.Features.Commands.ProductCommands
{
    public class UpdateProductCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Response<string>>
    {
        private readonly IGenericRepository<Product> _productRepository;

        public UpdateProductCommandHandler(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Response<string>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // First try to find it by ID (from BaseEntity)
            var product = await _productRepository.GetByIDAsync(request.Id);
            if (product == null)
            {
                return Response<string>.Fail("Product not found");
            }

            product.Name = request.Name;
            product.Category = request.Category;
            product.Price = request.Price;
            product.Quantity = request.Quantity;
            product.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return Response<string>.Success(null, "Product updated successfully");
        }
    }
}
