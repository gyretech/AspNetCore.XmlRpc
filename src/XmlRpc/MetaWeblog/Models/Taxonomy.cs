using System.Runtime.Serialization;

namespace AspNetCore.XmlRpc.MetaWeblog.Models
{
    public struct Taxonomy
    {
        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "label")]
        public string Label;

        [DataMember(Name = "hierarchical")]
        public bool Hierarchical;

        [DataMember(Name = "public")]
        public bool Public;

        [DataMember(Name = "show_ui")]
        public bool ShowUI;
    }
}