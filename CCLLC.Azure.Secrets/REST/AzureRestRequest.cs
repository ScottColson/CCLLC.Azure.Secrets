using System;
using System.Text;


namespace CCLLC.Azure.Secrets
{
    using CCLLC.Core.Net;
    using CCLLC.Core.RESTClient;
    using CCLLC.Core.Serialization;

    public abstract class AzureRestRequest<T> : SerializedRESTRequest<T> where T : class, ISerializedRESTResponse
    {
        protected AzureRestRequest(IDataSerializer serializer, IAPIEndpoint endpoint, string accessToken = null) 
            : base(serializer, endpoint, accessToken)
        {
            if (!this.ApiEndpoint.QueryParameters.ContainsKey("api-version"))
            {
                this.ApiEndpoint.AddQuery("api-version", "7.0");
            }
        }

        protected virtual string AuthenticationToken
        {
            get
            {
                var clearText = this.AccessToken;
                var encodedText = Convert.ToBase64String(Encoding.UTF8.GetBytes(clearText));
                return string.Format("Bearer {0}",encodedText);
            }
        }

        public override T Execute(IHttpWebRequest webRequest)
        {
            webRequest.Headers.Add("Authorization", this.AuthenticationToken);
            webRequest.Headers.Add("Cache-Control", "no-cache");

            return InternalExecute(this.Serializer, webRequest);
            
        }

       
        protected virtual T InternalExecute(IDataSerializer serializer, IHttpWebRequest webRequest)
        {
            var webResponse = webRequest.Get();
            return serializer.Deserialize<T>(webResponse.Content);
        }
    }
}
