using System;
using System.Reflection;

namespace HL7Interface.Tests
{
    public class BaseTests
    {
        protected const int timeout = 20000;
        protected void CreateClient()
        {

        }

        public BaseTests()
        {
            /* Preparing test start */
            Assembly assembly = Assembly.GetCallingAssembly();
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
            /* Preparing test end */
        }
    }
}