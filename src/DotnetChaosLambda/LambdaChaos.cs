using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotnetChaosLambda;

public class LambdaChaos
{
    private readonly Random _random;
    private ChaosConfig _config;
    
    public LambdaChaos()
    {
        this._random = new Random(DateTime.Now.Second);
    }

    public LambdaChaos(ChaosConfig init)
    {
        this._random = new Random(DateTime.Now.Second);
        this._config = init;
    }
    
    public async Task ExecuteChaos()
    {
        this.loadConfig();

        if (this._config == null)
        {
            return;
        }
        
        Console.WriteLine($"Got chaos config: {this._config}");

        switch (this._config.FaultType)
        {
            case "latency":
                await this.addLatency();
                return;
            case "exception":
                await this.addError();
                return;
        }
    }
    
    private async Task addError()
    {
        Console.WriteLine($"Injecting error");

        if (string.IsNullOrEmpty(this._config.ExceptionMsg) || this._config.Rate <= 0)
        {
            return;
        }

        var nextValue = this._random.NextDouble();
        
        Console.WriteLine($"Rate limiter is {nextValue}, rate is set to {this._config.Rate}");

        if (nextValue < this._config.Rate)
        {
            throw new Exception(this._config.ExceptionMsg);
        }
    }

    private async Task addLatency()
    {
        Console.WriteLine($"Injecting latency of {this._config.Delay}");

        if (this._config.Delay <= 0 || this._config.Rate <= 0)
        {
            return;
        }

        var nextValue = this._random.NextDouble();
        
        Console.WriteLine($"Random number is {nextValue}, rate is set to {this._config.Rate}");

        if (nextValue < this._config.Rate)
        {
            Console.WriteLine("Sleeping now");
            await Task.Delay(this._config.Delay);
        }

        Console.WriteLine("Sleep complete");
    }

    private void loadConfig()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CHAOS_PARAM")))
        {
            return;
        }

        this._config = JsonSerializer.Deserialize<ChaosConfig>(Environment.GetEnvironmentVariable("CHAOS_PARAM"));
    }
}