namespace DiplomacyEngine.Model;

public class TransitoryLocation : Location
{
    public Location PreviousLocation { get; }
    public Location NewLocation { get; }
    
    public TransitoryLocation(Location prevLocaiton, Location newLocation) : base(newLocation.Name, newLocation.Value, newLocation.Type)
    {
        PreviousLocation = prevLocaiton;
        NewLocation = newLocation;
    }
}