using HL7DeviceSimulator.StatePattern;
using Hl7Interface.ServerProtocol;
using HL7Interface;
using HL7Interface.Configuration;
using HL7Interface.ServerProtocol;

namespace HL7DeviceSimulator
{
    public class Device
    {
        internal State State;
        public HL7InterfaceBase HL7Interface;

        public void Init()
        {
            State = new Ready();

            HL7Interface = new HL7Interface.HL7InterfaceBase();

            HL7Server server = new HL7Server();

            HL7ProtocolBase protocol = new HL7ProtocolBase(new HL7ProtocolConfig()
            {
                IsAckRequired = false,
                IsResponseRequired = false
            });

            server.Setup("127.0.0.1", 2012);
            server.Start();

            HL7Server serverSide = new HL7Server();
            serverSide.Setup("127.0.0.1", 50060);

            HL7Interface.Initialize(serverSide, protocol);

            HL7Interface.Start();
        }
    }
}
