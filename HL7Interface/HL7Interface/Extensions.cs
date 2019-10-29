using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HL7Interface
{

    public static class Extensions
    {
        /// <summary>
        /// Wait the Handle,  timeout or cancellation token
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="timeout"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool Wait(this WaitHandle handle, int timeout, CancellationToken token)
        {
            var result = WaitHandle.WaitAny(new[] { handle, token.WaitHandle }, timeout);

            if (result == 0) return true;

            if (result == 1) throw new OperationCanceledException(token);

            return false;
        }
    }
}
