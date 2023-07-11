using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using HelloWorld.Shared;
using Moq;
using Xunit;

namespace HelloWorld.Tests;

public class FunctionTest
{
    [Fact]
    public async Task TestHelloWorldFunctionHandler()
    {
        var mockNetworkService = new Mock<INetworkService>();
        mockNetworkService.Setup(p => p.GetCallingIP()).ReturnsAsync("52.9.9.6");

        var body = new Dictionary<string, string>
            {
                { "message", "hello world" },
                { "location", "52.9.9.6" },
            };

        var expectedResponse = new APIGatewayProxyResponse
        {
            Body = JsonSerializer.Serialize(body),
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };

        var function = new Function(mockNetworkService.Object);

        var apiGatewayRequest =
            JsonSerializer.Deserialize<APIGatewayProxyRequest>(EventHelper.ApiGatewayRequest);
        // Or, instead to deserializing json, we could just do `var request = new APIGatewayProxyRequest();`

        var response = await function.FunctionHandler(apiGatewayRequest, new TestLambdaContext());

        Assert.Equal(expectedResponse.Body, response.Body);
        Assert.Equal(expectedResponse.Headers, response.Headers);
        Assert.Equal(expectedResponse.StatusCode, response.StatusCode);
    }

    [Fact]
    public async Task TestHelloWorldFunctionHandlerForInternalIP_ShouldReturn400()
    {
        var mockNetworkService = new Mock<INetworkService>();
        mockNetworkService.Setup(p => p.GetCallingIP()).ReturnsAsync("10.0.0.2");

        var expectedResponse = new APIGatewayProxyResponse
        {
            Body = null,
            StatusCode = 400,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };

        var function = new Function(mockNetworkService.Object);

        var apiGatewayRequest =
            JsonSerializer.Deserialize<APIGatewayProxyRequest>(EventHelper.ApiGatewayRequest);
        // Or, instead to deserializing json, we could just do `var request = new APIGatewayProxyRequest();`

        var response = await function.FunctionHandler(apiGatewayRequest, new TestLambdaContext());

        Assert.Equal(expectedResponse.Body, response.Body);
        Assert.Equal(expectedResponse.Headers, response.Headers);
        Assert.Equal(expectedResponse.StatusCode, response.StatusCode);
    }
}
