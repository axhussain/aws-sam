namespace HelloWorld.Shared;

public interface INetworkService
{
    Task<string> GetCallingIP();
}
