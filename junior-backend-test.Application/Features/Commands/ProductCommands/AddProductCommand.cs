using junior_backend_test.Application.Wapper;
using unior_backend_test.Domain.Interfaces;
using junior_backend_test.Domain.Model;
using MediatR;
using junior_backend_test.Domain.ProductsDtos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace junior_backend_test.Application.Features.Commands.ProductCommands
{
    public class AddProductCommand : IRequest<Response<ProductDto>>
    {
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Response<ProductDto>>
    {
        private readonly IGenericRepository<Product> _productRepository;

        public AddProductCommandHandler(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Response<ProductDto>> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                ID = Guid.NewGuid(),
                Name = request.Name,
                Category = request.Category,
                Price = request.Price,
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Set BaseEntity properties if required, e.g. ID
            product.ID = Guid.NewGuid(); 

            _productRepository.Add(product);
            await _productRepository.SaveChangesAsync();

            var dto = new ProductDto
            {
                Id = product.ID,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Quantity = product.Quantity
            };

            return Response<ProductDto>.Success(dto, "Product added successfully");
        }
    }
}
