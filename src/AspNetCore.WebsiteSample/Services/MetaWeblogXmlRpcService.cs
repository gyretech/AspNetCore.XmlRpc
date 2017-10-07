using AspNetCore.XmlRpc.MetaWeblog;
using AspNetCore.XmlRpc.MetaWeblog.Models;

namespace AspNetCore.XmlRpc.WebsiteSample.Services
{
    public class MetaWeblogXmlRpcService : IMetaWeblogXmlRpcService
    {
        public MetaWeblogXmlRpcService()
        {
        }

        [XmlRpcMethod("blogger.deletePost")]
        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            throw new System.NotImplementedException();
        }

        [XmlRpcMethod("metaWeblog.editPost")]
        public string EditPost(string postid, string username, string password, PostInfo post, bool publish)
        {
            throw new System.NotImplementedException();
        }

        [XmlRpcMethod("metaWeblog.getCategories")]
        public CategoryInfo[] GetCategories(string blogid, string username, string password)
        {
            throw new System.NotImplementedException();
        }

        [XmlRpcMethod("metaWeblog.getPost")]
        public PostInfo GetPost(string postid, string username, string password)
        {
            throw new System.NotImplementedException();
        }

        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        public PostInfo[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            throw new System.NotImplementedException();
        }

        [XmlRpcMethod("blogger.getUserInfo")]
        public UserInfo GetUserInfo(string key, string username, string password)
        {
            throw new System.NotImplementedException();
        }

        [XmlRpcMethod("blogger.getUsersBlogs")]
        public BlogInfo[] GetUsersBlogs(string key, string username, string password)
        {
            throw new System.NotImplementedException();
        }

        [XmlRpcMethod("metaWeblog.newMediaObject")]
        public MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            throw new System.NotImplementedException();
        }

        [XmlRpcMethod("metaWeblog.newPost")]
        public string NewPost(string blogid, string username, string password, PostInfo post, bool publish)
        {
            throw new System.NotImplementedException();
        }
    }
}
