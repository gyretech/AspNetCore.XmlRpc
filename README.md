# AspNetCore.XmlRpc
XML-RPC .NET Standard library with MetaWeblog implementations for ASP.NET Core 2.x

# Build Status
![build status](https://www.myget.org/BuildSource/Badge/aspnetcore_xmlrpc?identifier=083bc4fc-b05d-48f7-b614-c24e3ffcfd5a)

# Detail
Why do we need XML-RPC if we have advanced technology to use such as SOAP or WCF?
The answer is: some applications are still using XML-RPC based protocols to communicate. For example, Open Live Writer (formerly named Windows Live Writer) supports MetaWeblog APIs when publishing blogs.
This project is migrated from the .NET 2 version to support ASP.NET Core. A number of components have been refactored to follow .NET Core standards. However not all the components are re-written yet. 

# Documentation
Detailed documentation will be published at my website [Kosmisch.net](http://kosmisch.net/).

# Automated tests
to be added. 

# How to use
1. Install the package

2. Change appSettings
Change appSettings.json to add configurations:
`{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "XmlRpc": {
    "GenerateSummary": "true",
    "SummaryEndpoint": "/api/xmlrpc/summary",
    "RsdEndpoint": "/api/xmlrpc/rsd",
    "Endpoint": "/api/xmlrpc/endpoint",
    "EngineName": "AspNetCore.XmlRpc",
    "BlogIdTokenName": "blogId",
    "HomePageEndpointPattern": "/Blog/{blogId}",
    "ManifestEndpoint": "/api/xmlrpc/manifest"
  }
}`

3. Add services
.. 3.1
`
public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Configure XmlRpc
            services.Configure<XmlRpcOptions>(Configuration.GetSection("XmlRpc"));
            services.AddMetaWeblog<MetaWeblogXmlRpcService, DefaultMetaWeblogEndpointProvider>();

            services.AddMvc();
        }
`
..3.2 Create your own implmentation of XML-RPC services, for instance MetaWeblogXmlRpcService
`
using AspNetCore.XmlRpc.MetaWeblog;
using AspNetCore.XmlRpc.MetaWeblog.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AspNetCore.XmlRpc.WebsiteSample.Services
{
    public class MetaWeblogXmlRpcService : IMetaWeblogXmlRpcService
    {
        public MetaWeblogXmlRpcService()
        {
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

        [XmlRpcMethod("blogger.deletePost")]
        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            throw new NotImplementedException();
        }

AspNetCore.XmlRpc.WebsiteSample.csproj        [XmlRpcMethod("metaWeblog.editPost")]
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

`
4. Add middleware
`
  app.UseStaticFiles();

            // Use XmlRpc middleware
            app.UseMetaWeblog(xmlRpcOptions);
`
5. Add links to support auto-detection in client tools like Open Live Writer
`
@using Microsoft.Extensions.Options;
@using AspNetCore.XmlRpc;
@inject IOptions<XmlRpcOptions> options;

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - AspNetCore.WebsiteSample</title>

    <link rel="EditURI" type="application/rsd+xml" title="RSD" href="@string.Concat(options.Value.RsdEndpoint,'/', "TestBlog")"  />
    <link rel="wlwmanifest" type="application/wlwmanifest+xml" href="@string.Concat(options.Value.ManifestEndpoint,'/', "TestBlog")" />
    ......
`

6. Sample project
Please to project AspNetCore.XmlRpc.WebsiteSample.csproj in the solution
