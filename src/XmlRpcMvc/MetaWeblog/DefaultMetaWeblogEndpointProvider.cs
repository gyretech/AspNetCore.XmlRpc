using System.Collections.Generic;
using System.Threading.Tasks;

namespace XmlRpcMvc.MetaWeblog
{
    public class DefaultMetaWeblogEndpointProvider : IMetaWeblogEndpointProvider
    {

        public DefaultMetaWeblogEndpointProvider(IEnumerable<IXmlRpcHandler> handlers)
        {
            Handlers = handlers;
        }

        public IEnumerable<IXmlRpcHandler> Handlers { get; set; }


        public async Task<bool> ProcessAsync(XmlRpcContext context)
        {
            foreach (var handler in Handlers)
            {
                if (handler.CanProcess(context))
                {
                    await handler.ProcessRequestAsync(context);
                    return true;
                }
            }
            return false;
        }
    }
}
