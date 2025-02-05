﻿using Admin.Application.Common.Interfaces;
using Admin.Application.Common.Models;
using Admin.Application.Products.DTOs;
using Admin.Domain.Entities;
using Admin.Domain.ValueObjects;

using FluentValidation;

using MediatR;


namespace Admin.Application.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ShortDescription { get; init; }
    public string Sku { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "USD";
    public decimal? CompareAtPrice { get; init; }
    public int Stock { get; init; }
    public int? LowStockThreshold { get; init; }
    public string Status { get; init; } = "Draft";
    public string Visibility { get; init; } = "Hidden";
    public Guid CategoryId { get; init; }
    public Guid? SubCategoryId { get; init; }
    public List<ProductImageDto> Images { get; init; } = new();
    public List<ProductVariantDto> Variants { get; init; } = new();
    public List<ProductAttributeDto> Attributes { get; init; } = new();
    public ProductSeoDto? Seo { get; init; }
    public ProductDimensionsDto? Dimensions { get; init; }
    public List<string> Tags { get; init; } = new();
}


public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Validate category
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
                return Result<Guid>.Failure(new Error("Category.NotFound", $"Category with ID {command.CategoryId} was not found"));

            // Validate subcategory if provided
            Category? subCategory = null;
            if (command.SubCategoryId.HasValue)
            {
                subCategory = await _categoryRepository.GetByIdAsync(command.SubCategoryId.Value, cancellationToken);
                if (subCategory == null)
                    return Result<Guid>.Failure(new Error("SubCategory.NotFound", $"SubCategory with ID {command.SubCategoryId} was not found"));
            }

            // Create product
            var product = new Product(
                command.Name,
                command.Description,
                Money.From(command.Price),
                currency: command.Currency,
                sku: command.Sku,
                stock: command.Stock,
                categoryId: command.CategoryId,
                id: command.SubCategoryId,
                createdBy: _currentUser.Id);

            //Handle images
            foreach (var image in command.Images)
            {
                product.AddImage(image.Url, image.FileName, image.Size, _currentUser.Id);
            }

            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(product.Id);
        }
        catch (Exception ex) when (ex is not ValidationException)
        {
            // Log the exception here
            return Result<Guid>.Failure(new Error("Product.CreateFailed", "Failed to create product due to an unexpected error"));
        }
    }
}