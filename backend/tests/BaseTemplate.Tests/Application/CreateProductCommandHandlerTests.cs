using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Products.Commands.CreateProduct;
using BaseTemplate.Domain.Common;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace BaseTemplate.Tests.Application;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IDomainEventDispatcher> _domainEventDispatcher = new();
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _unitOfWork.Setup(u => u.Products).Returns(_productRepository.Object);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _handler = new CreateProductCommandHandler(
            _unitOfWork.Object,
            _domainEventDispatcher.Object,
            NullLogger<CreateProductCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WithValidCommand_PersistsProductAndDispatchesEvents()
    {
        var command = new CreateProductCommand("Widget", "WGT-1", 19.99m, "USD", "A sample widget");

        var id = await _handler.Handle(command, CancellationToken.None);

        id.Should().NotBeEmpty();
        _productRepository.Verify(r => r.AddAsync(It.Is<Product>(p => p.Name == "Widget"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _domainEventDispatcher.Verify(
            d => d.DispatchAndClearEvents(It.IsAny<IEnumerable<BaseEntity>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
