using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using DotNetOpenAuth.AspNet;
using MVCScopedClients.Utilities.Linkedin;
using Newtonsoft.Json;
using MVCScopedClients.Utilities;

namespace MVCScopedClients
{
    public class LinkedinScopedClient : IAuthenticationClient
    {
        private readonly string _apiKey;
        private readonly string _secretKey;
        private readonly string _scope;
        private string _redirectUrl;
        public LinkedinScopedClient(string apiKey, string secretKey, string scope)
        {
            _apiKey = apiKey;
            _secretKey = secretKey;
            _scope = scope;
        }
        public string ProviderName
        {
            get { return "Linkedin"; }
        }

        public void RequestAuthentication(HttpContextBase context, Uri returnUrl)
        {
            _redirectUrl = HttpUtility.UrlEncode(returnUrl.ToString());
            var url =
                "https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=" +
                _apiKey + "&scope=" + _scope + "&state=TEEMUTAPANILA6&redirect_uri=" + _redirectUrl;
            context.Response.Redirect(url);
        }

        private string GetAccessToken(string code)
        {
            var url = "https://www.linkedin.com/uas/oauth2/accessToken?grant_type=authorization_code" +
                      "&code=" + code +
                      "&redirect_uri=" + _redirectUrl +
                      "&client_id=" + _apiKey +
                      "&client_secret=" + _secretKey;
            var wc = new WebClient();
            var data = wc.DownloadString(url);
            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(data);
            return accessTokenResponse.access_token;
        }

        private static IDictionary<string, string> GetUserData(string accessToken)
        {
            var wc = new WebClient();
            var url =
                "https://api.linkedin.com/v1/people/~?format=json&oauth2_access_token=" + accessToken;
            var data = wc.DownloadString(url);
            // this dictionary must contains
            var userData = JsonConvert.DeserializeObject<BasicProfile>(data);
            var formattedUserData = new Dictionary<string, string>
                                        {
                                            {"profileUrl", userData.siteStandardProfileRequest.url},
                                            {"firstName", userData.firstName},
                                            {"lastName", userData.lastName},
                                            {"accesstoken", accessToken}
                                        };

            return formattedUserData;
        }

        public AuthenticationResult VerifyAuthentication(HttpContextBase context)
        {
            var code = context.Request.QueryString["code"];

            if (code == null)
                return new AuthenticationResult(false, ProviderName, null, null, null);

            var accessToken = GetAccessToken(code);

            var extraData = GetUserData(accessToken);

            var id = extraData["profileUrl"].Substring("id=", "&");
            var result = new AuthenticationResult(true, ProviderName, id, extraData["firstName"] + " " + extraData["lastName"], extraData);
            return result;
        }
    }
}
