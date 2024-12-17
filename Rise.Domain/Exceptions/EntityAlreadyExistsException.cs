namespace Rise.Domain.Exceptions;

public class EntityAlreadyExistsException(string name, string parameterName, object id) : ApplicationException($"{name} met {parameterName} {id} bestaat al.")

{
}
