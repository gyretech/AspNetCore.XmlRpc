using System.Runtime.Serialization;

namespace AspNetCore.XmlRpc.MetaWeblog.Models
{
    public class MediaObjectInfo
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
