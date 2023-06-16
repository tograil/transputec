namespace CrisesControl.Core.Incidents;

public record AckOption(int ResponseId, string ResponseLabel, int ResponseCode)
{
    public AckOption() : this(0,string.Empty,0)
    {
       
    }
}
