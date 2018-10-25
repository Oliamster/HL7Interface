using NHapi.Base.validation;

namespace HL7api.Validation
{
    internal class HL7apiConfigurableContext
    {
        private IValidationContext validationContext;

        public HL7apiConfigurableContext(IValidationContext validationContext)
        {
            this.validationContext = validationContext;
        }
    }
}