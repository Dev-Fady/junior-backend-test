using junior_backend_test.Application.Wapper;
using unior_backend_test.Domain.Interfaces;
using junior_backend_test.Domain.Model;
using junior_backend_test.Domain.ProductsDtos;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace junior_backend_test.Application.Features.Queries.ProductQueries
{
    public class GetProductByIdQuery : IRequest<Response<ProductDto>>
    {
        public Guid Id { get; set; }

        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Response<ProductDto>>
    {
        private readonly IGenericRepository<Product> _productRepository;

        public GetProductByIdQueryHandler(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Response<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIDAsync(request.Id);
            if (product == null)
            {
                return Response<ProductDto>.Fail("Product not found");
            }

            var dto = new ProductDto
            {
                Id = product.ID,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Quantity = product.Quantity
            };

            return Response<ProductDto>.Success(dto);
        }
    }
}
