using System.Collections.Immutable;
using AutoMapper;
using AutoMapper.Internal;
using AutoMapper.Mappers;
using Newtonsoft.Json;

namespace DiplomacyEngine.Model;

public class DiplomacyGame
{
    internal Mapper Mapper { get; }
    
    public DiplomacyMap _dg;
    internal DiplomacyMapDTO dgdto;
    
    public DiplomacyGame(StreamReader sr)
    {

                
        var config = new MapperConfiguration(cfg =>
            {
                //Convert to DTO
                cfg.CreateMap<Location, LocationDTO>();
                cfg.CreateMap<Neighbors, NeighborsDTO>()
                    .ForMember(dest => dest.Neighbors,
                        opt => opt.MapFrom(src => from n in src._neighbors.Values select n.GetKeyValuePair(false).Key));
                cfg.CreateMap<Team, TeamDTO>();
                cfg.CreateMap<IPrimaryKey, string>().ConvertUsing(src => src.GetKey(false));
                cfg.CreateMap<UnitType, UnitTypeDTO>();
                cfg.CreateMap<Unit, UnitDTO>();
                
                //Look at these do we need them?
                cfg.CreateMap<Location, KeyValuePair<string, Location>>().ConvertUsing(src => new KeyValuePair<string, Location>(src.Name, src));
                cfg.CreateMap(typeof(string), typeof(IKeyValue<>));
                
                //From DTO, assumes _dg has been initialized
                cfg.CreateMap<LocationTypeDTO, LocationType>().ReverseMap();
                cfg.CreateMap<NeighborsDTO, Neighbors>().ConvertUsing(src => _dg.GetLocation(src.CurLocation).Neighbors);
                cfg.CreateMap<TeamDTO, Team>().ConvertUsing(src => _dg.GetTeam(src.Name));
                cfg.CreateMap<LocationDTO, Location>().ConvertUsing(src => _dg.GetLocation(src.Name));
                cfg.CreateMap<UnitDTO, Unit>().ConvertUsing(src => _dg.GetTeam(src.Team).GetUnit(src.Id));
                cfg.CreateMap<UnitTypeDTO, UnitType>().ConvertUsing(src => _dg.GetUnitType(src.Name));
                cfg.CreateMap<string, KeyValuePair<string, Location>>().ConvertUsing(src => new KeyValuePair<string, Location>(src, _dg.GetLocation(src)));
                cfg.CreateMap<string, LocationType>().ConvertUsing(src => _dg.GetLocationType(src));
                cfg.CreateMap<string, Location>().ConvertUsing(src => _dg.GetLocation(src));
                cfg.CreateMap<KeyValuePair<string, Location>, string>().ConvertUsing(src => src.Key);
                // cfg.CreateMap<string, LocationTypeDTO>().ConvertUsing(src => dgdto.LocationTypes.Where(lt => lt.Name == src).First());
                
                //Used for json serialization
                cfg.CreateMap<DiplomacyMapDTO, DiplomacyMap>()
                    .ForMember(
                        dest => dest.LocationTypes,
                        opt =>
                        {
                            opt.SetMappingOrder(0);
                            opt.MapFrom(src =>
                                (from t in src.LocationTypes
                                    select new LocationType(t.Name, t.Capacity, null)));
                            //TODO fix this section
                        })
                    .ForMember(
                        dest => dest.UnitTypes, 
                        opt => opt.MapFrom(
                            src => 
                            from t in src.UnitTypes select new UnitType(t.Name, t.Value, new []{UnitType.Unit})
                        ))
                    // .ForMember(
                    //     dest => dest.Locations,
                    //     opt => opt.Ignore()
                    // )
                    .AfterMap((src, dest) =>
                    {
                        dest.Locations = (
                            from loc in src.Locations
                            select new Location(loc.Name, loc.Value, dest.GetLocationType(loc.Type))
                        ).ToImmutableList();

                        var v = from loc in src.Locations
                            select dest.GetLocation(loc.Name).Neighbors = new Neighbors(null,null);

                        foreach (var loc in src.Locations)
                        {
                            var neighbors =
                                from l in src.Neighbors
                                where l.CurLocation == loc.Name
                                from nl in l.Neighbors
                                select dest.GetLocation(nl);
                                
                            dest.GetLocation(loc.Name).Neighbors =
                                new Neighbors(dest.GetLocation(loc.Name), neighbors);
                        }

                        dest.Teams = (from t in src.Teams
                                select new Team(t.Name, t.Score,
                                    (from l in t.ControlledLocations select dest.GetLocation(l)).ToImmutableList()))
                            .ToImmutableList();

                        var units = (
                            from t in src.Teams
                            from u in t.Units
                            select dest.GetTeam(t.Name).CreateUnit(dest.GetUnitType(u.Type), dest.GetLocation(u.Location)));

                        foreach (Unit u in units)
                        {
                            //Maybe this if we need it later
                            // u.Team._units.Add(u);   
                            u.Location.OccupiedBy = u;
                        }
                        _dg = dest;
                    })
                    .ForAllOtherMembers(opt => opt.Ignore());
                cfg.CreateMap<DiplomacyMap, DiplomacyMapDTO>();
            }
        );

        using (JsonTextReader jsonReader = new JsonTextReader(sr))
        {
            JsonSerializer jjss = JsonSerializer.CreateDefault();
            dgdto = jjss.Deserialize<DiplomacyMapDTO>(jsonReader);
        }

        var mapper = new Mapper(config);
        Mapper = mapper;
        _dg = mapper.Map<DiplomacyMap>(dgdto);

        var londonDTO = mapper.Map<LocationDTO>(_dg.GetLocation("London"));

        var londonUnMapped = mapper.Map<Location>(londonDTO);

        var neighsDTO = mapper.Map<NeighborsDTO>(londonUnMapped.Neighbors, opt => opt.Items["Game"] = this);

        var neighsUnmapped = mapper.Map<Neighbors>(neighsDTO);

        var output = mapper.Map<DiplomacyMapDTO>(_dg);
        Console.WriteLine(londonDTO);
        // string jsonReferenceString = JsonConvert.SerializeObject(_dg, Formatting.Indented, new JsonSerializerSettings()
        // {
        //     PreserveReferencesHandling = PreserveReferencesHandling.All
        // });
    }
}