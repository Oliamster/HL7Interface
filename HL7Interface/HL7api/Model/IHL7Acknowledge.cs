namespace HL7api.Model
{
    public interface IHL7Acknowledge
    {
        bool IsAckForRequest(IHL7Message request); 
    }
}
