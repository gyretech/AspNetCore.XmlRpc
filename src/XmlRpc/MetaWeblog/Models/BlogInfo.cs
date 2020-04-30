using System.Runtime.Serialization;

namespace AspNetCore.XmlRpc.MetaWeblog.Models
{
    public class BlogInfo
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "blogid")]
        public string Blogid { get; set; }

        [DataMember(Name = "blogName")]
        public string BlogName { get; set; }

        [DataMember(Name = "isAdmin")] public bool IsAdmin { get; set; }
    }
}
