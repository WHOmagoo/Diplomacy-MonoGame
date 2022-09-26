using System.Runtime.CompilerServices;
using AutoMapper.Internal;
using DiplomacyEngine.Model.Orders;
using DiplomacyEngine.Model.Types;

namespace DiplomacyEngine.Model;
public class UnitType : IKeyValue<UnitType>, ITargetable
{
    public static UnitType Army { get; } =
        new(nameof(Army), 1, new[] {LocationType.Land, LocationType.Coastal, LocationType.OffMap});

    public static UnitType Navy { get; } = 
        new(nameof(Navy), 1, new[] {LocationType.Offshore, LocationType.Coastal, LocationType.OffMap});

    public static UnitType Unit { get; } = new(nameof(Unit), 1, new ITargetable[]{});
    
    
    public string Name { get; }
    public int Value { get; private set; }

    private HashSet<ITargetable> mVisits { get; }
    private HashSet<ITargetable> parentTypes { get; }
    
    public UnitType(string name, int value, IEnumerable<ITargetable> visits)
    {
        Name = name;
        Value = value;

        mVisits = new HashSet<ITargetable>();
        foreach (var visit in visits)
        {
            mVisits.Add(visit);
        }

        parentTypes = new HashSet<ITargetable>();
        parentTypes.Add(Unit);
    }
    
    public KeyValuePair<string, UnitType> GetKeyValuePair(bool includeType = false)
    {
        return new KeyValuePair<string, UnitType>(GetKey(includeType), this);
    }

    public string ToString()
    {
        return Name;
    }

    public string GetKey(bool includeType = false)
    {
        return includeType ? "UnitType:" + Name : Name;
    }

    public bool IsType(string type)
    {
        return type == Name;
    }

    public bool Visits(LocationType lt)
    {
        return mVisits.Contains(lt);
    }

    public int Capacity => Value;
    public bool IsType(ITargetable other)
    {
        return parentTypes.Overlaps(other.GetParentTypes());
    }

    public IEnumerable<ITargetable> GetParentTypes()
    {
        return parentTypes;
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

public class UnitTypeDTO
{
    public string Name { get; private set; }
    public int Value { get; private set; }

    private UnitTypeDTO()
    {
        
    }

    public UnitTypeDTO(string name, int value)
    {
        Name = name;
        Value = value;
    }
}