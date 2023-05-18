using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ViewerTestApp.Shared
{
    static public class Settings
    {
        static public string UrlBase
        {
            get { return "https://developer.api.autodesk.com/"; }
        }

        static public string AuthenticateSuffix
        {
            get { return "authentication/v1/authenticate"; }
        }

        static public string UploadFileSuffix
        {
            get { return "oss/v2/buckets/"; }
        }

        static public string TranslateJobSuffix
        {
            get { return "/api/forge/modelderivative/jobs"; }
        }

        static public string ManifestJobSuffix
        {
            get { return "modelderivative/v2/designdata/"; }
        }
        static public string WorkItemSuffix
        {
            get { return "da/us-east/v3/workitems"; }
        }
    }

    public class ForgeAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public DateTime expiration { get; set; }
        public bool valid
        {
            get
            {
                if (access_token.Length <= 0) return false;
                if (DateTime.Now >= expiration) return false;
                return true;
            }
        }
    }
    public static class Authenticate
    {
        public static async Task<ForgeAccessToken> GetAccessToken(string cId, string cSecret, string scope = "data:read data:write data:create bucket:read")
        {
            HttpClient client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Post, Settings.UrlBase + Settings.AuthenticateSuffix);
            //req.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials"},
                { "client_id", cId },
                { "client_secret", cSecret },
                { "scope", scope }
            });
            HttpResponseMessage resp = await client.SendAsync(req);
            ForgeAccessToken t = JsonConvert.DeserializeObject<ForgeAccessToken>(await resp.Content.ReadAsStringAsync());
            t.expiration = DateTime.Now.AddSeconds(t.expires_in);
            return t;
        }

    }
}
