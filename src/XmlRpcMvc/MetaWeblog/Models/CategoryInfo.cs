using System.Runtime.Serialization;

namespace XmlRpcMvc.MetaWeblog.Models
{
    public class CategoryInfo
    {
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}
