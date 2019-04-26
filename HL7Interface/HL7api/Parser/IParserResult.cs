using HL7api.Model;

namespace HL7api.Parser
{
    public interface IParserResult<TMessage>
    {
        /// <summary>
        /// The resulting acknowledgment
        /// </summary>
        TMessage Acknowledge { get; }

        /// <summary>
        /// The error message
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Whether the message is accepted
        /// </summary>
        bool MessageAccepted { get; set; }

        /// <summary>
        /// Whether this message is an ack
        /// </summary>
        bool? IsAcknowledge { get; }

        /// <summary>
        /// Parsed Message
        /// </summary>
        TMessage ParsedMessage { get; }
    }
}