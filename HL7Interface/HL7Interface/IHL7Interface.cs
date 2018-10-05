using HL7api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface
{
    public interface IHL7Interface
    {
        /// <summary>
        /// The interface name
        /// </summary>
        String Name { get;}

        HL7Protocol Protocol { get; }

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
        bool Initialise();
        /// <summary>
        /// because the interface  can run either as server o client, it need
        /// to be started.
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// stop the server side
        /// </summary>
        /// <returns></returns>
        bool Stop();
    }
}
