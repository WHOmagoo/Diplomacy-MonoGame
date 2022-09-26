using DiplomacyEngine.Model.Orders;
using DiplomacyEngine.Model.Types;

namespace DiplomacyEngine.Model;
public class Unit : IKeyValue<Unit>, ITargetable
{
    public Unit(UnitType type, Team team, Location location, string id)
    {
        Type = type;
        Team = team;
        Location = location;
        Id = id;
    }

    public UnitType Type {get;}
    public int PointCost => Type.Value;
    public Team Team { get; }
    public Location Location { get; }
    public string Id { get; }
    
    public Order Order { get; set; }

    private List<ITargetable> _itemsTargeting;

    public KeyValuePair<string, Unit> GetKeyValuePair(bool includeType = false)
    {
        return new KeyValuePair<string, Unit>(GetKey(includeType), this);
    }

    public string GetKey(bool includeType = false)
    {
        var team = Team.GetKey(false);
        var key = $"{team}:{Id}";
        key = includeType ? $"Unit:" + key : key;
        return key;
    }

    public int Capacity => PointCost;
    public bool IsType(ITargetable other)
    {
        return Type.IsType(other);
    }

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

    public IReadOnlyCollection<ITargetable> GetItemsTargeting()
    {
        return _itemsTargeting;
    }
}

public class UnitDTO
{
    public string Type {get;}
    public string Team { get; }
    public string Location { get; }
    
    public string Id { get; }

    public UnitDTO(string type, string team, string location, string id)
    {
        Type = type;
        Team = team;
        Location = location;
        Id = id;
    }
}