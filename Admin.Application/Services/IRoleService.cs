﻿namespace Admin.Application.Services;

public interface IRoleService
{
    IEnumerable<string> GetAvailableRoles();
    IEnumerable<string> GetRolePermissions(string role);
    bool IsValidRole(string role);
    bool HasRequiredPermissions(IEnumerable<string> userPermissions, string requiredPermission);
}
