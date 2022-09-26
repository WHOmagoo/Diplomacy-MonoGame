using System.IO;
using DiplomacyEngine.Model;
using NUnit.Framework;

namespace DiplomacyGameTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        using(var fs = new FileStream("/home/whomagoo/RiderProjects/MonoGameCross-PlatformDesktopApplication1/MonoGameCross-PlatformDesktopApplication1/Content/EuropeGameBoard.json", FileMode.Open))
        using (StreamReader sr = new StreamReader(fs))
        {
            var dg = new DiplomacyGame(sr);
            Assert.Pass();
        }
    }
}