using System.Collections.Generic;
using System.Linq;
using CCLLC.Core;
using CCLLC.Core.Net;
using CCLLC.Core.Serialization;

namespace CCLLC.Azure.Secrets
{ 
    public class AzureSecretProvider : SettingsProvider, ISecretProvider
    {
        private readonly Dictionary<string, string> Secrets;
        private readonly IJSONContractSerializer Serializer;
        private readonly string TenantId;
        private readonly string ClientId;
        private readonly string ClientSecret;
        private readonly string VaultName;
      
        public AzureSecretProvider(IReadOnlyDictionary<string,string> secretIds, IJSONContractSerializer serializer, string tenantId, string clientId, string clientSecret, string vaultName) : base(secretIds)
        {
            this.Serializer = serializer;
            this.TenantId = tenantId;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.VaultName = vaultName;
            
            this.Secrets = new Dictionary<string, string>();
        }


        public override string this[string key]
        {
            get
            {
                GetAllSecrets(base.KeyValuePairs);
                return Secrets[key];
            }
        }

        public override bool TryGetValue(string key, out string value)
        {
            if (base.ContainsKey(key) && !Secrets.ContainsKey(key))
            {
                GetSecret(key);
            }

            return Secrets.TryGetValue(key, out value);
        }

        public override IEnumerable<string> Values
        {
            get
            {
                GetAllSecrets(base.KeyValuePairs);
                return Secrets.Values;
            }
        }

        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            GetAllSecrets(base.KeyValuePairs);
            return Secrets.GetEnumerator();
        }

        private void GetAllSecrets(IReadOnlyDictionary<string, string> secretIds)
        {
            if (Secrets.Count < secretIds.Count)
            {
                var token = GetToken();

                foreach(var key in secretIds.Keys)
                {
                    if (!Secrets.ContainsKey(key))
                    {
                        GetSecret(key, token);
                    }
                }
            }
        }

        private void GetSecret(string key, AuthToken token = null)
        {
            if (token is null)
                token = GetToken();

            var versionRequest = new GetSecretVersionsRequest(Serializer, token, VaultName, key);

            var webRequest = new HttpWebRequestWrapper(versionRequest.ApiEndpoint);

            var versions = versionRequest.Execute(webRequest);

            if (versions is null || versions.value.Length == 0)
            {
                Secrets.Add(key, null);
                return;
            }

            var curentVersion  = versions.value
                .Where(v => v.attributes.enabled)
                .OrderByDescending(v => v.attributes.created)
                .FirstOrDefault()?.id;

            if (string.IsNullOrEmpty(curentVersion))
            {
                Secrets.Add(key, null);
                return;
            }

            var secretRequest = new GetSecretRequest(Serializer, token, curentVersion);
            webRequest = new HttpWebRequestWrapper(secretRequest.ApiEndpoint);
            var value = secretRequest.Execute(webRequest)?.value;

            Secrets.Add(key, value);
        }

        
        private AuthToken GetToken()
        {
            var request = new GetAuthTokenRequest(this.Serializer, TenantId, ClientId, ClientSecret);

            var webRequest = new HttpWebRequestWrapper(request.ApiEndpoint);

            return request.Execute(webRequest);
        }
    }
}
