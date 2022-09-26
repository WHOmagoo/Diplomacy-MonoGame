namespace DiplomacyEngine.Model.Targetables;

public class Coastal : LocationType
{
    public Coastal(IReadOnlyCollection<LocationType> locationTypesInheretedFrom = null) : base(nameof(Coastal), 1, locationTypesInheretedFrom)
    {
    }
}