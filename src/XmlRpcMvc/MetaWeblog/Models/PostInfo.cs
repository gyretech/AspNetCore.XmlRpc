using System;
using System.Runtime.Serialization;

namespace XmlRpcMvc.MetaWeblog.Models
{
    public class PostInfo
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "dateCreated")]
        public DateTime DateCreated { get; set; }

        [DataMember(Name = "categories")]
        public string[] Categories { get; set; }
    }
}
