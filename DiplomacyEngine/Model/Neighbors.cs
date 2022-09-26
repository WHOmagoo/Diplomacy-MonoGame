using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using DiplomacyEngine.Model.Types;

namespace DiplomacyEngine.Model;
public class Neighbors : IReadOnlyCollection<Location>, IKeyValue<Neighbors>
{
    public Dictionary<string, Location> _neighbors;

    private HashSet<Location> _neighborsHash;
    public Location CurLocation { get; private set; }
    
    public Neighbors()
    {
        _neighbors = new Dictionary<string, Location>();
    }

    public Neighbors(Location curLocation, IEnumerable<Location> neighbors) : this()
    {
        _neighborsHash = new HashSet<Location>();

        CurLocation = curLocation;
        foreach (var n in neighbors)
        {
            _neighbors[n.Name] = n;
            _neighborsHash.Add(n);
        }

        Location l = new Location("Hi", 1, null);

    }

    public Location GetNeighbor(string name)
    {
        if (Contains(name))
            return _neighbors[name];
        return null;
    }

    public bool Contains(string name)
    {
        return _neighbors.ContainsKey(name);
    }

    // public IEnumerable<Location> GetEnumerator()
    // {
    //     return _neighbors.Values.GetEnumerator();
    // }
    //
    // public IEnumerator GetEnumerator()
    // {
    //     throw new System.NotImplementedException();
    // }
    
    public IEnumerator<Location> GetEnumerator() => _neighborsHash.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _neighborsHash.GetEnumerator();

    public KeyValuePair<string, Neighbors> GetKeyValuePair(bool includeType = false)
    {
        return new KeyValuePair<string, Neighbors>(GetKey(includeType), this);
    }

    public string GetKey(bool includeType = false)
    {
        string key = CurLocation.GetKey(false);
        key = includeType ? "Neighbors:" + key : key;
        return key;
    }

    public IReadOnlyCollection<Location> GetNeighborsInCommon(Neighbors n)
    {
        return _neighborsHash.Union(n).ToImmutableHashSet();
    }
    
    public IEnumerable<Location> GetNeighborsInCommon(Location l)
    {
        return GetNeighborsInCommon(l.Neighbors);
    }

    public ImmutableHashSet<Location> GetNeighborsWithinDegree(int n)
    {
        if (n == 0)
        {
            return ImmutableHashSet<Location>.Empty;
        }

        if (n == 1)
        {
            return this.ToImmutableHashSet();
        }
        
        HashSet<Location> locations = new HashSet<Location>(this);
        locations.Add(CurLocation);
        
        IReadOnlyCollection<Location> curLevelLocations = this;

        for (int i = 1; i < n; i++)
        {
            curLevelLocations =
                (from neighbors in curLevelLocations
                from loc in neighbors.Neighbors
                where !locations.Contains(loc)
                select loc).ToImmutableList();
            
            locations.UnionWith(curLevelLocations);
        }

        locations.Remove(CurLocation);
        
        return locations.ToImmutableHashSet();
    }

    public int Count => _neighborsHash.Count;
}

public class NeighborsDTO
{
    public string CurLocation { get; private set; }
    
    public ICollection<string> Neighbors { get; private set; }

    private NeighborsDTO()
    {
        
    }
    
    public NeighborsDTO(string curLocation, ICollection<string> neighbors)
    {
        CurLocation = curLocation;
        Neighbors = neighbors;
    }
}