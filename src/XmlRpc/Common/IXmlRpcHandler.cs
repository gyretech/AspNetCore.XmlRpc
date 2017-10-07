using System.Threading.Tasks;

namespace AspNetCore.XmlRpc
{
    /// <summary>
    /// Handler to process different XmlRpc requests.
    /// </summary>
    public interface IXmlRpcHandler
    {
        /// <summary>
        /// Whether the handler can process the current request
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool CanProcess(XmlRpcContext context);

        /// <summary>
        /// Process the request and provide response.
        /// </summary>
        /// <param name="context"></param>
        Task ProcessRequestAsync(XmlRpcContext context);
    }
}
