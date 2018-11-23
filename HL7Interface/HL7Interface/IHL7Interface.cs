using HL7api.Model;
using HL7Interface.ServerProtocol;
using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface
{
    public interface IHL7Interface : ICommunication<IHL7Message>
    {
        /// <summary>
        /// The interface name
        /// </summary>
        String Name { get;}

        /// <summary>
        /// The HL7 communication protocol 
        /// </summary>
        IHL7Protocol Protocol { get; }

        /// <summary>
        /// Send the HL7Message to the remote system
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<HL7Request> SendHL7MessageAsync(IHL7Message message);

        /// <summary>
        /// connect to the actor's remote end point.
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <returns></returns>
        Task<bool> ConnectAsync(System.Net.EndPoint remoteEndPoint);

        /// <summary>
        /// Initialise the server side
        /// </summary>
        /// <returns></returns>
        bool Initialize();

        /// <summary>
        /// because the interface  can run either as server o client, it need
        /// to be started.
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// The server side state
        /// </summary>
        ServerState State { get; }

        /// <summary>
        /// Stop the server side
        /// </summary>
        /// <returns></returns>
        void Stop();
    }
}
