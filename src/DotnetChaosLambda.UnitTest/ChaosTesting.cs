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
            FaultType = "latency"
        };

        var wrapper = new LambdaChaos(config);

        var startTime = DateTime.Now;

        await wrapper.ExecuteChaos();
        
        var endTime = DateTime.Now;

        var executionSeconds = (endTime - startTime).Seconds;
        
        Assert.True(executionSeconds >= 5, $"Execution time is {executionSeconds}");
    }
    
    [Fact]
    public async Task AddLatency_TestRate()
    {
        var config = new ChaosConfig()
        {
            Delay = 5000,
            Rate = 0.2, // Using a rate of 0.2 means 20% of requests should have the delay
            FaultType = "latency"
        };

        var wrapper = new LambdaChaos(config);

        var executionTimes = new List<double>();

        for (var loopNumber = 0; loopNumber < 5; loopNumber++)
        {   
            var startTime = DateTime.Now;

            await wrapper.ExecuteChaos();
        
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
            FaultType = "exception"
        };

        var wrapper = new LambdaChaos(config);

        var errorCount = 0;

        for (var loopNumber = 0; loopNumber < 5; loopNumber++)
        {   
            var startTime = DateTime.Now;

            try
            {
                await wrapper.ExecuteChaos();
            }
            catch (Exception)
            {
                errorCount++;
            }
        }

        Assert.True(errorCount == 1, $"Error count was {errorCount}");
    }
}