﻿namespace Admin.WebAPI.Endpoints.ProductsVariant.Models;

public record ProductDimensionsRequest
{
    public decimal Weight { get; init; }
    public decimal Width { get; init; }
    public decimal Height { get; init; }
    public decimal Length { get; init; }
    public string Unit { get; init; } = "cm";
}
