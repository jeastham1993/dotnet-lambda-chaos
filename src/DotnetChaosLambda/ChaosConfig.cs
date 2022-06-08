using System.Text.Json;

namespace DotnetChaosLambda;
public class ChaosConfig
{
    public string FaultType { get; set; }
    
    public int Delay { get; set; }
    
    public bool IsEnabled { get; set; }
    
    public int ErrorCode { get; set; }
    
    public string ExceptionMsg { get; set; }
    
    public double Rate { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
