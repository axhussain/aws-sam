using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using HelloWorld.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace HelloWorld;

// From James Eastham's YouTube channel: https://www.youtube.com/playlist?list=PLCOG9xkUD90JWJrqI8S63_MEDIgtF6JFo
public class Function
{
    private readonly INetworkService _networkService;

    public Function() : this(null)
    {
    }

    internal Function(INetworkService networkService = null)
    {
        Startup.ConfigureServices();

        _networkService = networkService ?? Startup.Services.GetRequiredService<INetworkService>();
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
    {

        var location = await _networkService.GetCallingIP();

        if (location.StartsWith("10.0"))
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        var body = new Dictionary<string, string>
            {
                { "message", "hello world" },
                { "location", location }
            };

        return new APIGatewayProxyResponse
        {
            Body = JsonSerializer.Serialize(body),
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}
