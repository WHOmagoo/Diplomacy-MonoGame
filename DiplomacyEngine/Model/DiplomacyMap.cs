using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiplomacyEngine.Model;

public class DiplomacyMap
{
    public IReadOnlyCollection<Location> Locations
    {
        get { return _locations.Values; }
        internal set
        {
            _locations = new Dictionary<string, Location>(
                from l in value
                select l.GetKeyValuePair()
            );
        }
    }
    private Dictionary<string, Location> _locations;

    public IReadOnlyCollection<Neighbors> Neighbors => (from l in Locations select l.Neighbors).ToImmutableList();

    public IReadOnlyCollection<LocationType> LocationTypes
    {
        get { return _locationTypesLookup.Values; }
        internal set
        {
            _locationTypesLookup =
                new Dictionary<string, LocationType>(
                    from t in value
                    select new KeyValuePair<string, LocationType>(t.Name, t)
                );
        }
    }

    private Dictionary<string, LocationType> _locationTypesLookup;

    public IReadOnlyCollection<Team> Teams
    {
        get { return _teams.Values;}
        internal set
        {
            _teams = new Dictionary<string, Team>(
                from t in value
                select new KeyValuePair<string, Team>(t.Name, t)
            );
        } 
    }

    public IReadOnlyCollection<UnitType> UnitTypes { get => _unitTypes.Values.ToImmutableList(); set => _unitTypes = new Dictionary<string, UnitType>(from u in value select u.GetKeyValuePair()); }

    private Dictionary<string, UnitType> _unitTypes;
    
    private Dictionary<string, Team> _teams;

    
    
    public DiplomacyMap()
    {
        _locations = new Dictionary<string, Location>();
        _teams = new Dictionary<string, Team>();
    }
    
    // public static DiplomacyGame FromStream(StreamReader sr)
    // {
    //     using (JsonTextReader jsonReader = new JsonTextReader(sr))
    //     {
    //         JObject jo = JObject.Load(jsonReader);
    //         return FromJObject(jo);
    //     }
    // }

    public Location GetLocation(string name)
    {
        return _locations[name];
    }

    public LocationType GetLocationType(string type)
    {
        return _locationTypesLookup[type];
    }
    // public static DiplomacyGame FromJObject(JObject doc)
    // {
    //     DiplomacyGame result = new DiplomacyGame();
    //     
    //     var Locations = doc["Locations"];
    //
    //     var neighbors = new List<(Location, JArray)>();
    //     
    //     foreach (var location in Locations)
    //     {
    //         Location l = new Location();
    //         l.Name = location["Name"].ToString();
    //         l._locationType = new LocationType(location["Type"].ToString());
    //         l.Value = int.Parse(location["Value"].ToString());
    //
    //         result._locations[l.Name] = l;
    //         
    //         neighbors.Add((l, (JArray)location["Neighbors"]));
    //     }
    //
    //     foreach (var tup in neighbors)
    //     {
    //
    //         var neighborReferences = new List<Location>();
    //
    //         foreach (var neighborName in tup.Item2)
    //         {
    //             neighborReferences.Add(result._locations[neighborName.ToString()]);
    //         }
    //         
    //         tup.Item1.Neighbors = new Neighbors(tup.Item1, neighborReferences);
    //     }
    //
    //     return result;
    // }

    // public static DiplomacyGame FromString(string s)
    // {
    //     DiplomacyGame result = new DiplomacyGame();
    //
    //     var doc = JObject.Parse(s);
    //
    //     return FromJObject(doc);
    // }
    public UnitType GetUnitType(string uType)
    {
        return _unitTypes[uType];
    }

    public Team GetTeam(string tName)
    {
        return _teams[tName];
    }
}

public class DiplomacyMapDTO
{
    public IReadOnlyCollection<LocationTypeDTO> LocationTypes { get; private set; }
    public IReadOnlyCollection<LocationDTO> Locations { get; private set; }
    public IReadOnlyCollection<NeighborsDTO> Neighbors { get; private set; }
    public IReadOnlyCollection<UnitTypeDTO> UnitTypes { get; private set; }
    public IReadOnlyCollection<TeamDTO> Teams { get; private set; }

    private DiplomacyMapDTO()
    {
        
    }

    public DiplomacyMapDTO(IReadOnlyCollection<LocationTypeDTO> locationTypes, IReadOnlyCollection<LocationDTO> locations, IReadOnlyCollection<NeighborsDTO> neighbors, IReadOnlyCollection<UnitTypeDTO> unitTypes, IReadOnlyCollection<TeamDTO> teams)
    {
        LocationTypes = locationTypes;
        Locations = locations;
        Neighbors = neighbors;
        UnitTypes = unitTypes;
        Teams = teams;
    }
}