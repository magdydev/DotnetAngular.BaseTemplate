using Asp.Versioning;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Products.Commands.CreateProduct;
using BaseTemplate.Application.Products.Dtos;
using BaseTemplate.Application.Products.Queries.GetProductById;
using BaseTemplate.Application.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

/// <summary>
/// Sample controller demonstrating the pattern for new features: a thin
/// endpoint that only translates HTTP <-> commands/queries via the dispatcher.
/// No business logic belongs here.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class ProductsController(IDispatcher dispatcher) : ControllerBase
{
    /// <summary>Lists every non-deleted product.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await dispatcher.Send(new GetProductsQuery(), cancellationToken);
        return Ok(products);
    }

    /// <summary>Gets a single product by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await dispatcher.Send(new GetProductByIdQuery(id), cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>Creates a new product.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var id = await dispatcher.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id, version = "1.0" }, id);
    }
}
