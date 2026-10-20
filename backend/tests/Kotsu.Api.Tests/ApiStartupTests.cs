using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Kotsu.Api.Tests;

public class ApiStartupTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Api_WithNoEndpointsYet_StartsAndAnswersNotFound()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        using var response = await client.GetAsync(new Uri("/", UriKind.Relative));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
