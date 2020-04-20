namespace CCLLC.Azure.Secrets
{
    using CCLLC.Core.Net;
    using CCLLC.Core.Serialization;

    public class GetSecretVersionsRequest : AzureRestRequest<SecretList>
    {
        public GetSecretVersionsRequest(IJSONContractSerializer serializer, AuthToken token, string url) 
            : base(serializer, new APIEndpoint(url), token?.access_token)
        { 
        }

        public GetSecretVersionsRequest(IJSONContractSerializer serializer, AuthToken token, string vaultName, string secretName) 
            : this(serializer, token, string.Format("https://{0}.vault.azure.net/secrets/{1}/versions", vaultName, secretName))
        {            
        }
    }
}
