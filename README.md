# AspNetCore.XmlRpc
XML-RPC .NET Standard library with MetaWeblog implementations for ASP.NET Core 2.x

# Build Status
![build status](https://www.myget.org/BuildSource/Badge/aspnetcore_xmlrpc?identifier=b1c23e7e-9f1e-4d2c-ab5f-849a91ec4910)

# Detail
Why do we need XML-RPC if we have advanced technology to use such as SOAP or WCF?
The answer is: some applications are still using XML-RPC based protocols to communicate. For example, Open Live Writer (formerly named Windows Live Writer) supports MetaWeblog APIs when publishing blogs.
This project is migrated from the .NET 2 version to support ASP.NET Core. A number of components have been refactored to follow .NET Core standards. However not all the components are re-written yet. 

# Documentation
Detailed documentation will be published at my website [kontext.tech](http://kontext.tech/Blog/ToolsAndFrameworks/aspnetcorexmlrpc).

# Automated tests
to be added. 

# How to use
## 1. Install the package
### Nuget PM
`Install-Package AspNetCore.XmlRpc -Version 1.0.0-alpha.4`
### MyGet PM
Refer to [My Get Page](https://www.myget.org/feed/aspnetcore_xmlrpc/package/nuget/AspNetCore.XmlRpc)

## 2. Change appSettings
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

## 3. Add services
### 3.1 Register services
`public void ConfigureServices(IServiceCollection services)
       {
            services.AddOptions();
            // Configure XmlRpc
            services.Configure<XmlRpcOptions>(Configuration.GetSection("XmlRpc"));
            services.AddMetaWeblog<MetaWeblogXmlRpcService, DefaultMetaWeblogEndpointProvider>();

            services.AddMvc();
        }`

### 3.2 Create your own implmentation of XML-RPC services, for instance MetaWeblogXmlRpcService
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
    //... implement the interface
    }
}
`
## 4. Add middleware
`app.UseStaticFiles();
  // Use XmlRpc middleware
  app.UseMetaWeblog();
`
## 5. Add links to support auto-detection in client tools like Open Live Writer
`
<link rel="EditURI" type="application/rsd+xml" title="RSD" href="@string.Concat(options.Value.RsdEndpoint,'/', "TestBlog")"  />
<link rel="wlwmanifest" type="application/wlwmanifest+xml" href="@string.Concat(options.Value.ManifestEndpoint,'/', "TestBlog")" />
`

## 6. Sample project
Please to project AspNetCore.XmlRpc.WebsiteSample.csproj in the solution
