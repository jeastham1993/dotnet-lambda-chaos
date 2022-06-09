namespace DotnetChaosLambda;

public class RateLimiter
{
    private readonly Random _random;
    public RateLimiter()
    {
        this._random = new Random(DateTime.Now.Second);
    }
    
    public bool ShouldExecuteFor(ChaosConfig config)
    {
        var nextValue = this._random.NextDouble();
        
        Console.WriteLine($"Rate limiter is {nextValue}, rate is set to {config.Rate}");

        return nextValue < config.Rate;
    }
}