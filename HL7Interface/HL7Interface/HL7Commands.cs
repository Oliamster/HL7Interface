namespace HL7Interface
{
    //public class V251EquipmentCommandRequestMLLP : CommandBase<HL7Session, HL7Request>
    //{
    //    public override void ExecuteCommand(HL7Session session, HL7Request packageInfo)
    //    {
    //        EquipmentCommandResponse response = ExecuteCommand(packageInfo);
    //        byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(response.Encode()));

    //        HL7SocketServiceConfig config = session.AppServer.Config as HL7SocketServiceConfig;

    //        session.Send(bytesToSend, 0, bytesToSend.Length);
    //    }

    //    protected virtual EquipmentCommandResponse ExecuteCommand(HL7Request request)
    //    {
    //        return new EquipmentCommandResponse();
    //    }

    //    public override string Name => this.GetType().Name;
    //}
}
