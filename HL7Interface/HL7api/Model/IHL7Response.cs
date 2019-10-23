namespace HL7api.Model
{
    public interface IHL7Response
    {
        string RequestID { get; }

        //bool IsResponseForRequest(IHL7Message request);
    }  
}
