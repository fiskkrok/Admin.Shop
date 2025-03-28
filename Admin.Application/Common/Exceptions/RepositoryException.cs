﻿namespace Admin.Application.Common.Exceptions;
public class RepositoryException : AppException
{
    public RepositoryException(string message, Exception? innerException = null)
        : base("Repository.Error", message, innerException ?? new Exception())
    {
    }
}
