using System.Threading.Tasks;

namespace HL7Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface ICommunication<TMessage>
    {
        //Task<TMessage> SendHL7MessageAsync(TMessage message);
    }
}