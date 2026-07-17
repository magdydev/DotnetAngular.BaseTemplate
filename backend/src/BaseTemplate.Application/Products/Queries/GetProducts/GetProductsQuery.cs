using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Products.Dtos;

namespace BaseTemplate.Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery : IQuery<IReadOnlyList<ProductDto>>;
