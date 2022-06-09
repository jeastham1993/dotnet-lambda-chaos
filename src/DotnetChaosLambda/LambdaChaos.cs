using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace DotnetChaosLambda;

public class LambdaChaos
{
    private readonly ChaosCache _cache;
    private readonly RateLimiter _rateLimiter;

    public LambdaChaos()
    {
        this._cache = new ChaosCache();
        this._rateLimiter = new RateLimiter();
    }

    /// <summary>
    /// Unleash chaos on your Lambda function. On invoke the chaos configuration will be loaded
    /// from SSM Parameter Store and cached. The TTL of the cache can be configured as part of the
    /// cache config using the CacheTTL property.
    /// </summary>
    public async Task UnleashChaos()
    {
        var config = await this._cache.GetConfig();
        
        if (config == null)
        {
            return;
        }

        if (config.IsEnabled == false)
        {
            return;
        }
        
        Console.WriteLine($"Got chaos config: {config}");

        switch (config.FaultType)
        {
            case "latency":
                await this.AddLatency(config);
                return;
            case "exception":
                this.AddError(config);
                return;
        }
    }
    
    private void AddError(ChaosConfig config)
    {
        Console.WriteLine($"Injecting error");

        if (string.IsNullOrEmpty(config.ExceptionMsg) || config.Rate <= 0)
        {
            return;
        }

        if (this._rateLimiter.ShouldExecuteFor(config))
        {
            throw new Exception(config.ExceptionMsg);
        }
    }

    private async Task AddLatency(ChaosConfig config)
    {
        Console.WriteLine($"Injecting latency of {config.Delay}");

        if (config.Delay <= 0 || config.Rate <= 0)
        {
            return;
        }

        if (this._rateLimiter.ShouldExecuteFor(config))
        {
            Console.WriteLine("Sleeping now");
            await Task.Delay(config.Delay);
        }

        Console.WriteLine("Sleep complete");
    }
}