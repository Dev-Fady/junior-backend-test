using System;

namespace junior_backend_test.Domain.ProductsDtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
