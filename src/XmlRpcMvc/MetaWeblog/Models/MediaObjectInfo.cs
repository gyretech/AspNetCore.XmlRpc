using System.Runtime.Serialization;

namespace XmlRpcMvc.MetaWeblog.Models
{
    public class MediaObjectInfo
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
