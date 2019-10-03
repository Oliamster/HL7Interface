using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7api.V251.Message
{
    /// <summary>
    /// PRepare for specimen acquisition
    /// </summary>
    public class PrepareForSpecimenRequest : EquipmentCommandRequest
    {
        public PrepareForSpecimenRequest(string sid) : this(new string[] { sid })
        {
           
        }

        public PrepareForSpecimenRequest(params string[] sids)
        {
            try
            {
                this.SetValue("EQU-2", DateTime.Now.ToString("yyyyMMddHHmmss"));

                for (int i = 0; i < sids.Length; i++)
                {
                    this.SetValue($"COMMAND({i})/SPECIMEN_CONTAINER/SAC-3", sids[i]);
                    this.SetValue($"COMMAND({i})/ECD-3", "Y");
                    this.SetValue($"COMMAND({i})/ECD-2-1-1", "LO");
                }
            }
            catch (Exception)
            {
                throw new HL7apiException($"Unable to initialise a new instance of {GetType().Name} message");
            }
        }
    }
}
