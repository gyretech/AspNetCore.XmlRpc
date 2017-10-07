using AspNetCore.XmlRpc.MetaWeblog.Models;

namespace AspNetCore.XmlRpc.MetaWeblog
{
    /// <summary>
    /// Xml Rpc service for MetaWeblog
    /// </summary>
    public interface IMetaWeblogXmlRpcService : IXmlRpcService
    {
        [XmlRpcMethod("metaWeblog.newPost")]
        string NewPost(string blogid, string username, string password, PostInfo post, bool publish);

        [XmlRpcMethod("metaWeblog.editPost")]
        string EditPost(string postid, string username, string password, PostInfo post, bool publish);

        [XmlRpcMethod("metaWeblog.getPost")]
        PostInfo GetPost(string postid, string username, string password);

        [XmlRpcMethod("metaWeblog.getCategories")]
        CategoryInfo[] GetCategories(string blogid, string username, string password);

        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        PostInfo[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts);

        [XmlRpcMethod("metaWeblog.newMediaObject")]
        MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject);

        [XmlRpcMethod("blogger.deletePost")]
        bool DeletePost(string key, string postid, string username, string password, bool publish);

        [XmlRpcMethod("blogger.getUsersBlogs")]
        BlogInfo[] GetUsersBlogs(string key, string username, string password);

        [XmlRpcMethod("blogger.getUserInfo")]
        UserInfo GetUserInfo(string key, string username, string password);
    }
}
