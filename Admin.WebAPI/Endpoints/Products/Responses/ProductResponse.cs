﻿using Admin.Application.Products.DTOs;

namespace Admin.WebAPI.Endpoints.Products.Responses;

public record ProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int Stock { get; init; }
    public CategoryResponse Category { get; init; } = null!;
    public CategoryResponse? SubCategory { get; init; }
    public List<ProductImageResponse> Images { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public string? LastModifiedBy { get; init; }

    public static ProductResponse FromDto(ProductDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        return new ProductResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Currency = dto.Currency,
            Stock = dto.Stock,
            Category = CategoryResponse.FromDto(dto.Category),
            SubCategory = dto.SubCategory != null ? CategoryResponse.FromDto(dto.SubCategory) : null,
            Images = dto.Images?.Select(ProductImageResponse.FromDto).ToList() ?? new(),
            CreatedAt = dto.CreatedAt,
            CreatedBy = dto.CreatedBy,
            LastModifiedAt = dto.LastModifiedAt,
            LastModifiedBy = dto.LastModifiedBy
        };
    }

    public static ProductResponse Summary(ProductDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        return new ProductResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            Price = dto.Price,
            Currency = dto.Currency,
            Stock = dto.Stock,
            Category = CategoryResponse.FromDto(dto.Category),
            Images = dto.Images?.Take(1).Select(ProductImageResponse.FromDto).ToList() ?? new()
        };
    }
}
