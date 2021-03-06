namespace DotnetChaosLambda.UnitTest;

public class ChaosTesting
{
    [Fact]
    public async Task AddLatency_ShouldAddDelay()
    {
        var config = new ChaosConfig()
        {
            Delay = 5000,
            Rate = 10,
            FaultType = "latency",
            IsEnabled = true
        };

        var wrapper = new LambdaChaos(config);

        var startTime = DateTime.Now;

        await wrapper.UnleashChaos();
        
        var endTime = DateTime.Now;

        var executionSeconds = (endTime - startTime).Seconds;
        
        Assert.True(executionSeconds >= 5, $"Execution time is {executionSeconds}");
    }
    
    [Fact]
    public async Task AddLatency_TestRateLimiting_ShouldDelay20PercentOfInvokes()
    {
        var config = new ChaosConfig()
        {
            Delay = 5000,
            Rate = 0.2, // Using a rate of 0.2 means 20% of requests should have the delay
            FaultType = "latency",
            IsEnabled = true
        };

        var wrapper = new LambdaChaos(config);

        var executionTimes = new List<double>();

        for (var loopNumber = 0; loopNumber < 5; loopNumber++)
        {   
            var startTime = DateTime.Now;

            await wrapper.UnleashChaos();
        
            var endTime = DateTime.Now;

            var executionSeconds = (endTime - startTime).Seconds;
            
            executionTimes.Add(executionSeconds);
        }

        var delayedExecutionCount = executionTimes.Count(p => p >= 5);
        
        Assert.True(delayedExecutionCount == 1, $"Delayed execution count was {delayedExecutionCount}");
    }
    
    [Fact]
    public async Task AddException_ShouldError()
    {
        var config = new ChaosConfig()
        {
            ExceptionMsg = "This is the failure message",
            Rate = 0.2, // Using a rate of 0.2 means 20% of requests should have the delay
            FaultType = "exception",
            IsEnabled = true
        };

        var wrapper = new LambdaChaos(config);

        var errorCount = 0;

        for (var loopNumber = 0; loopNumber < 100; loopNumber++)
        {   
            var startTime = DateTime.Now;

            try
            {
                await wrapper.UnleashChaos();
            }
            catch (Exception)
            {
                errorCount++;
            }
        }

        var errorPercentage = (1 - (100.00 - errorCount) / 100.00) * 100;

        Assert.True(errorPercentage >= 15 && errorPercentage <= 25, $"Error percentage was {errorPercentage}");
    }
}