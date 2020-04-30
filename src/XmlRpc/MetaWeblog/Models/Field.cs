using System.Runtime.Serialization;

namespace AspNetCore.XmlRpc.MetaWeblog.Models
{
    public struct Field
    {
        [DataMember(Name = "name")]
        public string Name { get; set; } 
    }
}