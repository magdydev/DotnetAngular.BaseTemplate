using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Products.Dtos;

namespace BaseTemplate.Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductDto?>;
