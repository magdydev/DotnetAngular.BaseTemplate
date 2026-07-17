namespace BaseTemplate.Application.Common.Exceptions;

/// <summary>Thrown when a use case looks up an entity by id and it does not exist.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" ({key}) was not found.")
    {
    }
}
