namespace CCLLC.Azure.Secrets
{
    using CCLLC.Core.Net;
    using CCLLC.Core.Serialization;

    public class GetSecretRequest : AzureRestRequest<SecretBundle>
    {
        public GetSecretRequest(IJSONContractSerializer serializer, AuthToken token, string secretId) 
            : base(serializer, new APIEndpoint(secretId), token?.access_token)
        {            
        }

        
    }
}
