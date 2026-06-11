using junior_backend_test.Application.Extentions.Pagination;
using junior_backend_test.Application.Wapper;
using unior_backend_test.Domain.Interfaces;
using junior_backend_test.Domain.Model;
using junior_backend_test.Domain.ProductsDtos;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace junior_backend_test.Application.Features.Queries.ProductQueries
{
    public class GetAllProductsQuery : PaginateBaseParamter, IRequest<Response<PaginatedList<ProductDto>>>
    {
        public GetAllProductsQuery() : base(1, 10)
        {
        }
    }

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Response<PaginatedList<ProductDto>>>
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMemoryCache _cache;

        public GetAllProductsQueryHandler(IGenericRepository<Product> productRepository, IMemoryCache cache)
        {
            _productRepository = productRepository;
            _cache = cache;
        }

        public async Task<Response<PaginatedList<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"ProductsList_{request.PageNumber}_{request.PageSize}";

            if (!_cache.TryGetValue(cacheKey, out Response<PaginatedList<ProductDto>> cachedData))
            {
                // Ordered by Price to take advantage of the newly created index
                var query = _productRepository.GetAll().OrderBy(p => p.Price);

                var paginatedData = await query
                    .Select(p => new ProductDto
                    {
                        Id = p.ID,
                        Name = p.Name,
                        Category = p.Category,
                        Price = p.Price,
                        Quantity = p.Quantity
                    })
                    .PaginateAsync(request.PageNumber, request.PageSize, cancellationToken);

                cachedData = Response<PaginatedList<ProductDto>>.Success(paginatedData);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Cache will expire if not accessed for 5 minutes
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Absolute expiration of 30 minutes

                _cache.Set(cacheKey, cachedData, cacheOptions);
            }

            return cachedData;
        }
    }
}
