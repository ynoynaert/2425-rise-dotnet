namespace Rise.Domain.Exceptions;

public class EntityNotFoundException(string name, object id) : ApplicationException($"{name} ({id}) werd niet gevonden.")
{
}
