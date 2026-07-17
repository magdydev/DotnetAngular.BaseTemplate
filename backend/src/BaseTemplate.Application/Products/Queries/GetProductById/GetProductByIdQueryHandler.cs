using AutoMapper;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Products.Dtos;
using BaseTemplate.Domain.Repositories;

namespace BaseTemplate.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(query.Id, cancellationToken);
        return product is null ? null : mapper.Map<ProductDto>(product);
    }
}
