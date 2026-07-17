using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Repositories;
using BaseTemplate.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<CreateProductCommandHandler> logger)
    : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var price = Money.Create(command.Price, command.Currency);
        var product = Product.Create(command.Name, command.Sku, price, command.Description);

        await unitOfWork.Products.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await domainEventDispatcher.DispatchAndClearEvents([product], cancellationToken);

        logger.LogInformation("Created product {ProductId} with SKU {Sku}", product.Id, product.Sku);

        return product.Id;
    }
}
