using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.XmlRpc.MetaWeblog
{
    public interface IMetaWeblogEndpointProvider
    {
        IEnumerable<IXmlRpcHandler> Handlers { get; set; }

        Task<bool> ProcessAsync(XmlRpcContext context);
    }
}
