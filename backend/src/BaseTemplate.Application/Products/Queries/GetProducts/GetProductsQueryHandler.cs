using AutoMapper;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Products.Dtos;
using BaseTemplate.Domain.Repositories;

namespace BaseTemplate.Application.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await unitOfWork.Products.ListAllAsync(cancellationToken);
        return mapper.Map<IReadOnlyList<ProductDto>>(products);
    }
}
