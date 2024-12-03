﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class UrlIgnoreListTests
    {
        private readonly string UrlToIgnore = "/ignore-me";
        private readonly string UrlWontIgnore = "/do-not-ignore-me";
        private readonly TestServer TestServer;

        public UrlIgnoreListTests()
        {
            var host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddRouting();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();
                            app.UseSecureHeadersMiddleware(urlIgnoreList: [UrlToIgnore]);
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapGet(UrlToIgnore, () =>
                                    TypedResults.Text("Hello Tests"));
                                endpoints.MapGet(UrlWontIgnore, () =>
                                    TypedResults.Text("Hello Tests"));
                            });
                        });
                })
                .Start();
        
            TestServer = host.GetTestServer();
            TestServer.BaseAddress = new Uri("https://example.com/");
        }

        [Fact]
        public async Task Invoke_IgnoreList_Contains_TargetUrl_NoHeadersAdded()
        {
            // arrange
        
            // Act
            var context = await TestServer.SendAsync(c =>
            {
                c.Request.Path = UrlToIgnore;
                c.Request.Method = HttpMethods.Get;
            });
            
            // Assert
            Assert.NotNull(context.Response);
            Assert.NotNull(context.Response.Headers);
            
            // Checking none of the default headers are present
            Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.StrictTransportSecurityHeaderName);
            Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.XFrameOptionsHeaderName);
            Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.XssProtectionHeaderName);
            Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.XContentTypeOptionsHeaderName);
            Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.ContentSecurityPolicyHeaderName);
            Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.PermittedCrossDomainPoliciesHeaderName);
            Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.ReferrerPolicyHeaderName);
            Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.CacheControlHeaderName);
        }
        
        [Fact]
        public async Task Invoke_IgnoreList_DoesntContain_TargetUrl_NoHeadersAdded()
        {
            // arrange
        
            // Act
            var context = await TestServer.SendAsync(c =>
            {
                c.Request.Path = UrlWontIgnore;
                c.Request.Method = HttpMethods.Get;
            });
            
            // Assert
            Assert.NotNull(context.Response);
            Assert.NotNull(context.Response.Headers);
            
            // Checking none of the default headers are present
            Assert.Contains(context.Response.Headers, h => h.Key == Constants.StrictTransportSecurityHeaderName);
            Assert.Contains(context.Response.Headers, h => h.Key == Constants.XFrameOptionsHeaderName);
            Assert.Contains(context.Response.Headers, h => h.Key == Constants.XssProtectionHeaderName);
            Assert.Contains(context.Response.Headers, h => h.Key == Constants.XContentTypeOptionsHeaderName);
            Assert.Contains(context.Response.Headers, h => h.Key == Constants.ContentSecurityPolicyHeaderName);
            Assert.Contains(context.Response.Headers, h => h.Key == Constants.PermittedCrossDomainPoliciesHeaderName);
            Assert.Contains(context.Response.Headers, h => h.Key == Constants.ReferrerPolicyHeaderName);
            Assert.Contains(context.Response.Headers, h => h.Key == Constants.CacheControlHeaderName);
        }
    }
}
