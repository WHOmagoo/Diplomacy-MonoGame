using System.Linq;
using System.Runtime.CompilerServices;
using DiplomacyEngine.Model.Orders;
using DiplomacyEngine.Model.Types;
using Newtonsoft.Json;

namespace DiplomacyEngine.Model;
public class Location : IKeyValue<Location>, ITargetable
{
    public string Name { get; set; }
    
    public Neighbors Neighbors { get; set; }
    
    public Unit? OccupiedBy { get; internal set; }

    public LocationType Type { get; internal set; }

    public int Value { get; set; }

    Location()
    {
        
    }
    public Location(string name, int value, LocationType locationType)
    {
        Name = name;
        Value = value;
        Type = locationType;
    }

    public KeyValuePair<string, Location> GetKeyValuePair(bool includeType = false)
    {
        return new KeyValuePair<string, Location>(GetKey(includeType), this);
    }

    public string GetKey(bool includeType = false)
    {
        return includeType ? "Location:" + Name : Name;
    }

    public int Capacity => Type.Capacity;
    public bool IsType(ITargetable other) => Type.IsType(other);
    public IEnumerable<ITargetable> GetParentTypes()
    {
        return Type.GetParentTypes();
    }

    public void AddTargetedBy(Order order)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Order> GetTargetedBy()
    {
        throw new NotImplementedException();
    }
}

public class LocationDTO
{
    public string Name { get; private set; }
    public int Value { get; private set; }
    public string Type { get; private set; }

    public LocationDTO(string name, int value, string type)
    {
        Name = name;
        Value = value;
        Type = type;
    }
}