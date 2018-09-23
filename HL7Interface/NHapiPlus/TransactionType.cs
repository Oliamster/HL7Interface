using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHapiPlus
{
    public enum TransactionType
    {
        /// <summary>
        /// this is a request
        /// </summary>
        Request,

        /// <summary>
        /// A sollicited transaction
        /// </summary>
        SollicitedUpdate,

        /// <summary>
        /// An unsollicited transaction
        /// </summary>
        UnsollicitedUpdate,

        /// <summary>
        /// An acknowledgment
        /// </summary>
        Acknowledgment
    }
}
