using System.Collections.Generic;

namespace AspNetCore.XmlRpc
{
    internal class XmlRpcRequest
    {
        public string MethodName { get; set; }
        public List<object> Parameters { get; set; }
    }
}