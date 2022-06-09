using System.Net;
using System.Text.Json;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace DotnetChaosLambda;

public class ChaosCache
{
    private ChaosConfig? _config;
    private DateTime _lastLoad;
    private readonly AmazonSimpleSystemsManagementClient _ssmClient;

    public ChaosCache()
    {
        this._ssmClient = new AmazonSimpleSystemsManagementClient();
        this.loadConfig().Wait();
    }

    public async Task<ChaosConfig?> GetConfig()
    {
        Console.WriteLine("Retrieving config from cache");
        
        if (this._config == null)
        {
            Console.WriteLine("Cache is empty, returning null");
            
            return this._config;
        }

        var secondsSinceLastCacheRefresh = (DateTime.Now - this._lastLoad).TotalSeconds;
        
        if (secondsSinceLastCacheRefresh > this._config.CacheTTL)
        {
            Console.WriteLine("Refreshing config");
            await this.loadConfig();
        }

        Console.WriteLine($"Cached value used. It has been {secondsSinceLastCacheRefresh} since the last cache refresh");

        return this._config;
    }
    
    private async Task loadConfig()
    {
        try
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CHAOS_PARAM")))
            {
                return;
            }

            Console.WriteLine("Starting chaos retrieval");

            var configValue = await this._ssmClient.GetParameterAsync(new GetParameterRequest()
            {
                Name = Environment.GetEnvironmentVariable("CHAOS_PARAM") 
            });
        
            Console.Write($"Retrieved chaos param {configValue.HttpStatusCode}");
        
            if (configValue == null || configValue.HttpStatusCode != HttpStatusCode.OK)
            {
                return;
            }

            this._config = JsonSerializer.Deserialize<ChaosConfig>(configValue.Parameter.Value);

            this._lastLoad = DateTime.Now;

            Console.WriteLine("Successfully parsed chaos");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            throw;
        }
    }
}