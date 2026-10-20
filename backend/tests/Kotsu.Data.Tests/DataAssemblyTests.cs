using System.Reflection;

namespace Kotsu.Data.Tests;

public class DataAssemblyTests
{
    [Fact]
    public void DataAssembly_WithNoTypesYet_BuildsAndLoads()
    {
        // Arrange
        var name = new AssemblyName("Kotsu.Data");

        // Act
        var assembly = Assembly.Load(name);

        // Assert
        Assert.Equal("Kotsu.Data", assembly.GetName().Name);
    }
}
