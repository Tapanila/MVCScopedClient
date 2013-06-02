using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using DotNetOpenAuth.AspNet;
using MVCScopedClients.Utilities.Facebook;
using Newtonsoft.Json;
using MVCScopedClients.Utilities;

namespace MVCScopedClients
{
    public class FacebookScopedClient : IAuthenticationClient
    {
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly string _scope;

        private const string BaseUrl = "https://www.facebook.com/dialog/oauth?client_id=";
        private const string GraphApiToken = "https://graph.facebook.com/oauth/access_token?";
        private const string GraphApiMe = "https://graph.facebook.com/me?";

        public FacebookScopedClient(string appId, string appSecret, string scope)
        {
            _appId = appId;
            _appSecret = appSecret;
            _scope = scope;
        }
        public FacebookScopedClient(string appId, string appSecret)
        {
            _appId = appId;
            _appSecret = appSecret;
            _scope = "email";
        }


        private static string GetHtml(string url)
        {
            var connectionString = url.Replace("%3a80", "");

            using (var wc = new WebClient())
            {
                return wc.DownloadString(connectionString);
            }
        }

        private IDictionary<string, string> GetUserData(string accessCode, string redirectUri)
        {
            var token = GetHtml(GraphApiToken + "client_id=" + _appId + "&redirect_uri=" + HttpUtility.UrlEncode(redirectUri) + "&client_secret=" + _appSecret + "&code=" + accessCode);
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            var data = GetHtml(GraphApiMe + "fields=id,name,email,username,gender,link,picture.type(large)&access_token=" + token.Substring("access_token=", "&"));
            var userData = JsonConvert.DeserializeObject<LoginData>(data);

            var formattedUserData = new Dictionary<string, string>
                                        {
                                            {"id", userData.id},
                                            {"name", userData.name},
                                            {"username", userData.username},
                                            {"gender", userData.gender},
                                            {"link", userData.link},
                                            {"picture", userData.Picture.Data.url},
                                            {"email", userData.email},
                                            {"accesstoken", token.Substring("access_token=", "&")}
                                        };

            return formattedUserData;
        }


        public string ProviderName
        {
            get { return "Facebook"; }
        }

        public string DisplayName { get { return ProviderName; } }

        public void RequestAuthentication(HttpContextBase context, Uri returnUrl)
        {
            var url = BaseUrl + _appId + "&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.ToString()) + "&scope=" + _scope;
            context.Response.Redirect(url);
        }

        public AuthenticationResult VerifyAuthentication(HttpContextBase context)
        {
            var code = context.Request.QueryString["code"];


            var rawUrl = context.Request.Url.OriginalString;
            rawUrl = Regex.Replace(rawUrl, "&code=[^&]*", "");

            var userData = GetUserData(code, rawUrl);

            if (userData == null)
                return new AuthenticationResult(false, ProviderName, null, null, null);

            var id = userData["id"];
            var username = userData["username"];
            userData.Remove("id");
            userData.Remove("username");

            return new AuthenticationResult(true, ProviderName, id, username, userData);
        }
    }
}