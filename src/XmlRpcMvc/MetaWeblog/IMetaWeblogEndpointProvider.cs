using System.Collections.Generic;
using System.Threading.Tasks;

namespace XmlRpcMvc.MetaWeblog
{
    public interface IMetaWeblogEndpointProvider
    {
        IEnumerable<IXmlRpcHandler> Handlers { get; set; }

        Task<bool> ProcessAsync(XmlRpcContext context);
    }
}
