using AspNetCore.XmlRpc.MetaWeblog;
using AspNetCore.XmlRpc.MetaWeblog.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AspNetCore.XmlRpc.WebsiteSample.Services
{
    public class MetaWeblogXmlRpcService : IMetaWeblogXmlRpcService
    {
        public MetaWeblogXmlRpcService(IOptions<XmlRpcOptions> options)
        {
            this.options = options;
        }

        UserInfo user = new UserInfo()
        {
            Email = "test@notexistingdomain.com",
            FirstName = "Test",
            LastName = "AspNetCore",
            Url = "http://notexistingdomain.com/users/1",
            UserId = "1",
            NickName = "Test"
        };

        CategoryInfo[] categories = new CategoryInfo[] { new CategoryInfo()
        {
             Title="Test Cate 1",
             Description="Test Cate 1 Desc"
        }
        };

        BlogInfo blog = new BlogInfo()
        {
            Blogid = "TestBlog",
            BlogName = "Test Blog",
            Url = "http://notexistingdomain.com/Blog/TestBlog"
        };

        Collection<PostInfo> posts = new Collection<PostInfo>();
        private readonly IOptions<XmlRpcOptions> options;

        [XmlRpcMethod("blogger.deletePost")]
        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            throw new NotImplementedException();
        }

        [XmlRpcMethod("metaWeblog.editPost")]
        public string EditPost(string postid, string username, string password, PostInfo post, bool publish)
        {
            throw new NotImplementedException();
        }

        [XmlRpcMethod("metaWeblog.getCategories")]
        public CategoryInfo[] GetCategories(string blogid, string username, string password)
        {
            return categories;
        }

        [XmlRpcMethod("metaWeblog.getPost")]
        public PostInfo GetPost(string postid, string username, string password)
        {
            throw new NotImplementedException();
        }

        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        public PostInfo[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            return posts.Take(numberOfPosts).ToArray();
        }

        [XmlRpcMethod("blogger.getUserInfo")]
        public UserInfo GetUserInfo(string key, string username, string password)
        {
            return user;
        }

        [XmlRpcMethod("blogger.getUsersBlogs")]
        public BlogInfo[] GetUsersBlogs(string key, string username, string password)
        {
            return new BlogInfo[] { blog };
        }

        [XmlRpcMethod("metaWeblog.newMediaObject")]
        public MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            throw new NotImplementedException();
        }

        [XmlRpcMethod("metaWeblog.newPost")]
        public string NewPost(string blogid, string username, string password, PostInfo post, bool publish)
        {
            posts.Add(post);
            return new Guid().ToString();
        }
    }
}
