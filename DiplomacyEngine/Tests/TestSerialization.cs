using DiplomacyEngine.Model;
using NUnit.Framework;

namespace DiplomacyEngine.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestReferencePersistanceDuringSerialization()
    {
        using(var fs = new FileStream("/home/whomagoo/RiderProjects/MonoGameCross-PlatformDesktopApplication1/MonoGameCross-PlatformDesktopApplication1/Content/EuropeGameBoard.json", FileMode.Open))
        using (StreamReader sr = new StreamReader(fs))
        {
            var dg = new DiplomacyGame(sr);
            var m = dg.Mapper;

            var original = dg._dg;
            // ref DiplomacyMap 
            //     = ref original;
            // var v = m.Map<DiplomacyMapDTO>(original);
            // var remapped = m.Map<DiplomacyMap>(v);
            // ref DiplomacyMap hi1 = ref remapped;

            foreach (var l in original.Locations)
            {
                var dto = m.Map<LocationDTO>(l);
                var l2 = m.Map<Location>(dto);
                Assert.AreSame(l, l2);
            }
            
            foreach (var n in original.Neighbors)
            {
                var dto = m.Map<NeighborsDTO>(n);
                var n2 = m.Map<Neighbors>(dto);
                Assert.AreSame(n, n2);
            }

            foreach (var t in original.Teams)
            {
                var dto = m.Map<TeamDTO>(t);
                var t2 = m.Map<Team>(dto);
                Assert.AreSame(t, t2);
            }
        }
    }
    
    // private static void PrintAllTypes(Type currentType, string prefix, HashSet<ref> visited)
    // {
    //     if (alreadyVisitedTypes.Contains(currentType)) return;
    //     alreadyVisitedTypes.Add(currentType);
    //     foreach (PropertyInfo pi in currentType.GetProperties())
    //     {
    //         Console.WriteLine($"{prefix} {pi.PropertyType.Name} {pi.Name}");
    //         if (!pi.PropertyType.IsPrimitive) PrintAllTypes(pi.PropertyType, prefix + "  ");
    //     }
    // }
}