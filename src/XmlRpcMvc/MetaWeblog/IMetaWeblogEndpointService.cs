using System.Threading.Tasks;

namespace XmlRpcMvc.MetaWeblog
{
    public interface IMetaWeblogEndpointService
    {
        /// <summary>
        /// Endpoint about blog API
        /// </summary>
        /// <param name="blogID"></param>
        /// <returns></returns>
        Task Rsd(string blogID);

        /// <summary>
        /// Endpoint to configure 
        /// </summary>
        /// <param name="blogID"></param>
        /// <returns></returns>
        Task Endpoint(string blogID);
    }
}
