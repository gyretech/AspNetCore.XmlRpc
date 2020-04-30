using System.Runtime.Serialization;

namespace AspNetCore.XmlRpc.MetaWeblog.Models
{
    public struct PostType
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "hierarchical")]
        public bool Hierarchical { get; set; }

        [DataMember(Name = "public")]
        public bool Public { get; set; }

        [DataMember(Name = "show_ui")]
        public bool ShowUi { get; set; }

        [DataMember(Name = "_builtin")]
        public bool BuiltIn { get; set; }

        [DataMember(Name = "has_archive")]
        public bool HasArchive { get; set; }

        [DataMember(Name = "menu_icon")]
        public string MenuIcon { get; set; }

        [DataMember(Name = "show_in_menu")]
        public bool ShowInMenu { get; set; }
    }
}