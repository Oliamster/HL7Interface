﻿using NUnit.Framework;
using System;
using System.Net;
using System.Reflection;

namespace HL7Interface.Tests
{
    public class BaseTests
    {
        protected const int timeout = 500000;
        protected System.Net.EndPoint clientEndPoint;
        protected System.Net.EndPoint serverEndpoint;

        
        [SetUp]
        public void TestInitialize()
        {
            clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50050);
            serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50060);

        }

        [TearDown]
        public void TestTearDown()
        {

        }

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