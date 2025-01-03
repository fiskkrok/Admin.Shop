﻿namespace Admin.WebAPI.Endpoints.ProductsVariant;

public record ProductSeoRequest
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public List<string> Keywords { get; init; } = new();
}