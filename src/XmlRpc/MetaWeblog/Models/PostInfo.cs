using System;
using System.Runtime.Serialization;

namespace AspNetCore.XmlRpc.MetaWeblog.Models
{
    public class PostInfo
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "dateCreated")]
        public DateTime DateCreated { get; set; }

        [DataMember(Name = "date_modified")]
        public DateTime DateModified { get; set; }

        [DataMember(Name = "categories")]
        public string[] Categories { get; set; }

        [DataMember(Name = "mt_excerpt")]
        public string Excerpt { get; set; }

        [DataMember(Name = "mt_text_more")]
        public string ReadMore { get; set; }

        [DataMember(Name = "wp_more_text")]
        public string WpReadMore { get; set; }

        [DataMember(Name = "wp_slug")]
        public string Slug { get; set; }

        [DataMember(Name = "link")] 
        public string Link { get; set; }

        [DataMember(Name = "mt_keywords")]
        public string KeyWords { get; set; }

        [DataMember(Name = "postid")]
        public string PostId { get; set; }
    }
}
