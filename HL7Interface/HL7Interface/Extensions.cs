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

        public static async Task<bool> WaitAsync(this Task task, int timeout, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
               
                var result = task.Wait(timeout, token);
            }
            catch (Exception ex)
            {

                throw;
            }

            //if (await Task.WhenAny(task, Task.Delay(timeout, token)) == task)
            //{
            //    token.ThrowIfCancellationRequested();

            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return  await new TaskCompletionSource<bool>().Task;
        }
    }
}
