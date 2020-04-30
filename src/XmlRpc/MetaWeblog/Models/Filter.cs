using System.Runtime.Serialization;

namespace AspNetCore.XmlRpc.MetaWeblog.Models
{
    public struct Filter
    {
        [DataMember(Name = "number")] public int Number { get; set; }

        [DataMember(Name = "offset")] public int OffSet { get; set; }

        [DataMember(Name = "orderby")] public string OrderBy { get; set; }

        [DataMember(Name = "order")] public string Order { get; set; }

        [DataMember(Name = "hide_empty")] public string HideEmpty { get; set; }

        [DataMember(Name = "search")] public string Search { get; set; }
    }
}