﻿namespace Admin.WebAPI.Endpoints.Categories.Response;

public record CategoryBreadcrumbResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public int Level { get; init; }
}
