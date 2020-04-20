using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCLLC.Azure.Secrets.UnitTest
{
    using CCLLC.Core;
    using CCLLC.Core.Net;
    using CCLLC.Core.Serialization;
    using System.Collections.Generic;

    [TestClass]
    public class AzureSecretProviderTest
    {
        struct TestData
        {
            public static readonly string TenantId = "fd4c6cf3-91d6-49b9-93a2-1b02518577cf";
            public static readonly string ClientId = "482696a8-f61b-431a-b3bf-b7d83aba64e3";
            public static readonly string ClientSecret = "IOxi[0Lyd6MBB1U0:e.yptYFBhS/d@jV";
            public static readonly string VaultName = "ccllctestvault";
        }

        class FakeExecutionContext : IProcessExecutionContext
        {

            private ISettingsProvider settingsProvider;

            public FakeExecutionContext()
            {
                var settings = new Dictionary<string, string>();

                settings.Add("CCLLC.Azure.Secrets.TenantId", TestData.TenantId);
                settings.Add("CCLLC.Azure.Secrets.ClientId", TestData.ClientId);
                settings.Add("CCLLC.Azure.Secrets.ClientSecret", TestData.ClientSecret);
                settings.Add("CCLLC.Azure.Secrets.VaultName", TestData.VaultName);

                settingsProvider = new SettingsProvider(settings);
            }
        
            

            public IDataService DataService => throw new NotImplementedException();

            public ICache Cache => null;

            public Core.IReadOnlyIocContainer Container => throw new NotImplementedException();

            public ISettingsProvider Settings => settingsProvider;

            public void Trace(string message, params object[] args)
            {
                throw new NotImplementedException();
            }

            public void Trace(eSeverityLevel severityLevel, string message, params object[] args)
            {
                throw new NotImplementedException();
            }

            public void TrackEvent(string name)
            {
                throw new NotImplementedException();
            }

            public void TrackException(Exception ex)
            {
                throw new NotImplementedException();
            }
        }


        [TestMethod]
        public void Test_SecretProviderFactory()
        {
            var webRequestFactory = new WebRequestFactory();
            var serializer = new DefaultJSONSerializer();

            var secretProviderFactory = new AzureSecretProviderFactory(webRequestFactory, serializer);

            var executionContext = new FakeExecutionContext();

            var provider = secretProviderFactory.Create(executionContext);

            Assert.AreEqual(26, provider.Count);

            var value = provider.GetValue<string>("MySimpleSecret");

            Assert.AreEqual("This is a secret", value);
        }
    }
}
