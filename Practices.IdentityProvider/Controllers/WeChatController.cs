namespace Practices.IdentityProvider.Controllers {
    using System;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class WeChatController : Controller {
        string corpId = ConfigurationManager.AppSettings["WeChat.CorpId"];        
        string corpSecret = ConfigurationManager.AppSettings["WeChat.CorpSecret"];
        string appId = ConfigurationManager.AppSettings["WeChat.AppId"];
        string appSecret = ConfigurationManager.AppSettings["WeChat.AppSecret"];
        string state = new Random().Next(1000, 200000).ToString();

        public ApplicationSignInManager SignInManager {
            get {
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        public ActionResult Post(string returnUrl) {
            var host = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority);
            returnUrl = string.Format("{0}/WeChat/Consumer?ReturnUrl={1}", host, HttpUtility.UrlEncode(returnUrl));
            var url = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&agentid={3}&state={4}#wechat_redirect",
                corpId, HttpUtility.UrlEncode(returnUrl), "snsapi_base", appId, state);
            return Redirect(url);
        }

        //[Route("WeChat/Consumer/{returnUrl}/{routefix?}")]
        public ActionResult Consumer(string code, string state, string returnUrl) {
            var host = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority);
            returnUrl = returnUrl.Replace(host, "");
            var access_token = GetAccessToken();
            var url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}", access_token, code);
            var jsonString = RequestUrl(url);
            var userid = GetJsonValue(jsonString, "UserId");
            var userName = userid + "@contoso.com";
            var result = SignInManager.PasswordSignIn(userName, password: "", isPersistent: false, shouldLockout: false);
            switch (result) {
                case SignInStatus.Success:
                    if (Url.IsLocalUrl(returnUrl)) {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View();
            }
        }

        protected string GetAccessToken() {
            var url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", corpId, appSecret);
            var jsonString = RequestUrl(url);
            var access_token = GetJsonValue(jsonString, "access_token");
            return access_token;
        }

        public static string RequestUrl(string url) {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = "GET";
            request.ContentType = "text/html";
            request.Headers.Add("charset", "utf-8");
            
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream, Encoding.Default);
            string content = sr.ReadToEnd();
            return content;
        }

        public static string GetJsonValue(string jsonStr, string key) {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(jsonStr)) {
                key = "\"" + key.Trim('"') + "\"";
                int index = jsonStr.IndexOf(key) + key.Length + 1;
                if (index > key.Length + 1) {
                    //先截逗号，若是最后一个，截“｝”号，取最小值
                    int end = jsonStr.IndexOf(',', index);
                    if (end == -1) {
                        end = jsonStr.IndexOf('}', index);
                    }

                    result = jsonStr.Substring(index, end - index);
                    result = result.Trim(new char[] { '"', ' ', '\'' }); //过滤引号或空格
                }
            }
            return result;
        }
    }
}