namespace AspNetCore.XmlRpc
{
    /// <summary>
    /// Configurations for Xml Rpc
    /// </summary>
    public class XmlRpcOptions
    {
        public XmlRpcOptions()
        {

        }

        public bool GenerateSummary { get; set; } = true;

        public string SummaryEndpoint { get; set; } = "/api/xmlrpc/summary";

        public string RsdEndpoint { get; set; } = "/api/xmlrpc/rsd";

        public string Endpoint { get; set; } = "/api/xmlrpc/endpoint";

        public string EngineName { get; set; } = "AspNetCore.XmlRpc";

        public string HomePageEndpointPattern { get; set; } = "/Blog/{blogId}";

        public string BlogIdTokenName { get; set; } = "blogId";
    }
}
