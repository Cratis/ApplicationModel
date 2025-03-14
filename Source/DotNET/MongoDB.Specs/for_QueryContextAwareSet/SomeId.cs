using Cratis.Concepts;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet;

public record SomeId(Guid value) : ConceptAs<Guid>(value)
{
    public static implicit operator SomeId(Guid value) => new(value);
}