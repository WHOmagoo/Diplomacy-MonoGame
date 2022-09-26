using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualBasic.CompilerServices;

namespace DiplomacyEngine.Model;
public class Team : IKeyValue<Team>
{
    public Team(string name, int score, IReadOnlyCollection<Location> controlled)
    {
        Name = name;
        Score = score;
        ControlledLocations = controlled;
    }

    public string Name { get; }
    public int Score { get; protected set; }

    public IReadOnlyCollection<Location> ControlledLocations
    {
        get => _controlledLocations.Values;
        internal set => _controlledLocations = new Dictionary<string, Location>(from l in value select l.GetKeyValuePair());
    }
    
    private Dictionary<string, Location> _controlledLocations = new Dictionary<string, Location>();
    
    public IReadOnlyCollection<Unit> Units
    {
        get => _units.Values.ToImmutableList();
    }

    public Unit GetUnit(string id)
    {
        return _units[id];
    }
    
    private Dictionary<string, Unit> _units = new Dictionary<string, Unit>();

    private int _curIdIndex = 0;
    
    public KeyValuePair<string, Team> GetKeyValuePair(bool includeType = false)
    {
        return new KeyValuePair<string, Team>(GetKey(includeType), this);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        if (obj is string)
        {
            return Name == (string) obj;
        }

        return false;
    }

    public string GetKey(bool includeType = false)
    {
        return includeType ? "Team:" + Name : Name;
    }

    public static bool operator ==(Team obj1, object? obj2)
    {
        if (obj1 is null)
        {
            if (obj2 == null)
            {
                return true;
            }
            return false;
        }

        Team t = obj2 as Team;
        if (t is null)
        {
            return false;
        }
        
        return obj1.Name == t.Name;
    }

    public static bool operator !=(Team obj1, object? obj2)
    {
        return !(obj1 == obj2);
    }

    public Unit CreateUnit(UnitType type, Location location)
    {
        string unitKey;
        do
        {
            unchecked
            {
                _curIdIndex++;   
            }

            unitKey = $"{GetKey()}:{_curIdIndex}";

        } while (_units.ContainsKey(unitKey));

        return CreateUnit(type, location, unitKey);
    }

    //TODO look into efficacy of refactoring this method into a builder
    public Unit CreateUnit(UnitType type, Location location, string id)
    {
        //TODO handle key collisions?
        var unit = new Unit(type, this, location, id);
        _units[unit.GetKey()] = unit;
        return unit;
    }
}

public class TeamDTO
{
    public string Name { get; private set; } 
    public ICollection<string> ControlledLocations { get; private set; }
    public int Score { get; private set; }
    public ICollection<UnitDTO> Units{ get; private set; }

    private TeamDTO()
    {
        
    }
    
    public TeamDTO(string name, ICollection<string> controlledLocations, int score, ICollection<UnitDTO> units)
    {
        Name = name;
        ControlledLocations = controlledLocations;
        Score = score;
        Units = units;
    }
}