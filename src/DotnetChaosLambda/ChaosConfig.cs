using System.Text.Json;

namespace DotnetChaosLambda;
public class ChaosConfig
{
    public ChaosConfig()
    {
        this.FaultType = string.Empty;
        this.ExceptionMsg = string.Empty;
        this.CacheTTL = TimeSpan.FromMinutes(1).TotalSeconds;
    }
    
    public string FaultType { get; set; }
    
    public int Delay { get; set; }
    
    public bool IsEnabled { get; set; }
    
    public int ErrorCode { get; set; }
    
    public string ExceptionMsg { get; set; }
    
    public double Rate { get; set; }
    
    public double CacheTTL { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
