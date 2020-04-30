using System;
using AspNetCore.XmlRpc.MetaWeblog.Models;

namespace AspNetCore.XmlRpc.MetaWeblog
{
    /// <summary>
    /// Xml Rpc service for MetaWeblog
    /// </summary>
    public interface IMetaWeblogXmlRpcService : IXmlRpcService
    {
        // Meta Weblog Support

        [XmlRpcMethod("metaWeblog.newPost")]
        string NewPost(string blogid, string username, string password, PostInfo post, bool publish);

        [XmlRpcMethod("metaWeblog.editPost")]
        bool EditPost(string postid, string username, string password, PostInfo post, bool publish);

        [XmlRpcMethod("metaWeblog.getPost")]
        PostInfo GetPost(string postid, string username, string password);

        [XmlRpcMethod("metaWeblog.getCategories")]
        CategoryInfo[] GetCategories(string blogid, string username, string password);

        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        PostInfo[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts);

        [XmlRpcMethod("metaWeblog.newMediaObject")]
        MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject);

        // Blogger Support

        [XmlRpcMethod("blogger.deletePost")]
        bool DeletePost(string key, string postid, string username, string password, bool publish);

        [XmlRpcMethod("blogger.getUsersBlogs")]
        BlogInfo[] GetUsersBlogs(string key, string username, string password);

        [XmlRpcMethod("blogger.getUserInfo")]
        UserInfo GetUserInfo(string key, string username, string password);

        // MovableType Support

        [XmlRpcMethod("mt.publishPost", "Publish an existing post.")]
        bool PublishPost(string postid, string username, string password);

        [XmlRpcMethod("mt.getRecentPostTitles", "Retrieve the post titles of recent posts.")]
        PostInfo[] GetRecentPostTitles(string blogid, string username, string password);

        [XmlRpcMethod("mt.getCategoryList", "Retrieve list of all categories.")]
        CategoryInfo[] GetCategoryList(string blogId, string username, string password);

        [XmlRpcMethod("mt.getPostCategories", "Retrieve list of categories assigned to a post.")]
        CategoryInfo[] GetPostCategories(string postId, string username, string password);

        [XmlRpcMethod("mt.setPostCategories", "Sets the categories for a post.")]
        bool SetPostCategories(string postId, string username, string password, CategoryInfo[] categoryInfos);


        // WordPress Support
        [XmlRpcMethod("wp.getUsersBlogs","Returns information about all the blogs a given user is a member of.")]
        BlogInfo[] GetWpUsersBlogs(string username, string password);

        [XmlRpcMethod("wp.getTerms")]
        CategoryInfo[] GetWpCategories(string blogid, string username, string password, string taxonomy, Filter filter);
        
        [XmlRpcMethod("wp.getCategories")]
        CategoryInfo[] GetWpCategories(string blogid, string username, string password);

        [XmlRpcMethod("wp.getPostTypes")]
        PostType[] GetWpPostTypes(string blogid, string username, string password, Filter filter, String[] fields);

    }
}
