using System.Collections.Immutable;
using AutoMapper.Configuration.Conventions;
using DiplomacyEngine.Model.Orders;
using DiplomacyEngine.Model.Types;

namespace DiplomacyEngine.Model;

// public class CompoundLocationType : LocationType, IKeyValue<CompoundLocationType>
// {
//     private ICollection<LocationType> _locationTypes;
//     
//     public KeyValuePair<string, CompoundLocationType> GetKeyValuePair(bool includeType = false)
//     {
//         return new KeyValuePair<string, CompoundLocationType>(GetKey(), this);
//     }
//
//     public CompoundLocationType(string name, ICollection<LocationType> locationTypesInheretedFrom) : base(name)
//     {
//         
//     }
//
//     public new bool IsType(string type)
//     {
//         return base.IsType(type) || _locationTypes.Any(t => IsType(type));
//     }
// }
public class LocationType : IKeyValue<LocationType>, ITargetable
{
    public static LocationType Location { get; } = new("Location", 1, Enumerable.Empty<LocationType>());
    public static LocationType Land { get;  } = new("Land", 1, new []{Location});
    public static LocationType Offshore { get;  }= new("Offshore", 1, new []{Location});
    public static LocationType Coastal { get; } = new("Coastal", 1, new []{Location});
    public static LocationType OffMap { get; } = new("Off Map", -1, new[] {Location});
    
    public int Capacity { get; }
    public bool IsType(ITargetable other)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ITargetable> GetParentTypes()
    {
        return _locationTypes;
    }

    public void AddTargetedBy(Order order)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Order> GetTargetedBy()
    {
        throw new NotImplementedException();
    }

    public string Name { get; private set; }

    private HashSet<ITargetable> _locationTypes;
    

    public LocationType(string name, int capacity, IEnumerable<LocationType> locationTypesInheretedFrom)
    {
        Name = name;
        Capacity = capacity;

        _locationTypes = new HashSet<ITargetable>();
        if (locationTypesInheretedFrom != null)
        {
            foreach (var subType in locationTypesInheretedFrom)
            {
                _locationTypes.UnionWith(subType.GetParentTypes());
            }
        }
    }

    public KeyValuePair<string, LocationType> GetKeyValuePair(bool includeType = false)
    {
        return new KeyValuePair<string, LocationType>(GetKey(includeType), this);
    }

    
    public string ToString()
    {
        return Name;
    }

    public string GetKey(bool includeType = false)
    {
        return includeType ? "LocationType:" + Name : Name;
    }

    public bool IsType(LocationType other)
    {
        //TODO investigate if ReferenceEquals is correct here
        return ReferenceEquals(this, other) || _locationTypes.Contains(other);
    }
    
    // public bool IsType(string type)
    // {
    //     return type == Name || _locationTypes.Any(t => t.IsType(type));
    // }

    public string Domain()
    {
        return nameof(LocationType);
    }

    public bool IsType(ITypeValidator other)
    {
        return other.Domain() == this.Domain();
    }
}

public class LocationTypeDTO
{
    public string Name { get; private set; }
    public int Capacity { get; private set; }

    public IReadOnlyCollection<string> InheritedFrom;

    public LocationTypeDTO(string name, int capacity, IReadOnlyCollection<string> inheritedFrom = null)
    {
        Name = name;
        Capacity = capacity;
        InheritedFrom = inheritedFrom;
    }
}