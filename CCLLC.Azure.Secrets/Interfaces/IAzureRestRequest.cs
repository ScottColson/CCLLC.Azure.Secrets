using CCLLC.Core.Serialization;

namespace CCLLC.Azure.Secrets
{
    public interface IAzureRestRequest<T> where T : class, IAzureRestResponse
    {
        T Execute(IDataSerializer serializer);
    }
}
