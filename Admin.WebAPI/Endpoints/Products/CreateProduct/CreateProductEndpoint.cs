﻿using Admin.Application.Products.Commands.CreateProduct;
using Admin.WebAPI.Endpoints.Products.GetProductById;
using FastEndpoints;

using MediatR;

namespace Admin.WebAPI.Endpoints.Products.CreateProduct;

public class CreateProductEndpoint : Endpoint<CreateProductCommand, Guid>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateProductEndpoint> _logger;

    public CreateProductEndpoint(IMediator mediator, ILogger<CreateProductEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/products");
        Description(d => d
            .WithTags("Products")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateProduct")
            .WithOpenApi());

        //Claims("products.create");
        Policies("ProductsCreate", "FullAdminAccess");
    }

    public override async Task HandleAsync(CreateProductCommand req, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(req, ct);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Product created successfully with ID: {ProductId}", result.Value);
                await SendCreatedAtAsync<GetProductEndpoint>(
                    new { id = result.Value },
                    result.Value,
                    generateAbsoluteUrl: true,
                    cancellation: ct);
            }
            else
            {
                _logger.LogWarning("Failed to create product: {Errors}", string.Join(", ", result.Error?.Message));
                await SendErrorsAsync(int.TryParse(result.Error?.Code, default, out _) ? int.Parse(result.Error?.Code) : 400, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            await SendErrorsAsync(500, ct);
        }
    }
}
