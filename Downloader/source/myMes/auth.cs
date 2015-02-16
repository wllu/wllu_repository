using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;
using System.Linq;
namespace myMes
{
    class auth
    {
        private enum VkontakteScopeList
        {
            /// <summary>
            /// Пользователь разрешил отправлять ему уведомления. 
            /// </summary>
            notify = 1,
            /// <summary>
            /// Доступ к друзьям.
            /// </summary>
            friends = 2,
            /// <summary>
            /// Доступ к фотографиям. 
            /// </summary>
            photos = 4,
            /// <summary>
            /// Доступ к аудиозаписям. 
            /// </summary>
            audio = 8,
            /// <summary>
            /// Доступ к видеозаписям. 
            /// </summary>
            video = 16,
            /// <summary>
            /// Доступ к предложениям (устаревшие методы). 
            /// </summary>
            offers = 32,
            /// <summary>
            /// Доступ к вопросам (устаревшие методы). 
            /// </summary>
            questions = 64,
            /// <summary>
            /// Доступ к wiki-страницам. 
            /// </summary>
            pages = 128,
            /// <summary>
            /// Добавление ссылки на приложение в меню слева.
            /// </summary>
            link = 256,
            /// <summary>
            /// Доступ заметкам пользователя. 
            /// </summary>
            notes = 2048,
            /// <summary>
            /// (для Standalone-приложений) Доступ к расширенным методам работы с сообщениями. 
            /// </summary>
            messages = 4096,
            /// <summary>
            /// Доступ к обычным и расширенным методам работы со стеной. 
            /// </summary>
            wall = 8192,
            /// <summary>
            /// Доступ к документам пользователя.
            /// </summary>
            docs = 131072
        }    // список прав
        private int appId = 4686359; //id приложения
        string accessToken;
        string userId;
        private int scope = (int)(VkontakteScopeList.audio | VkontakteScopeList.docs | VkontakteScopeList.friends | VkontakteScopeList.link | VkontakteScopeList.messages | VkontakteScopeList.notes | VkontakteScopeList.notify | VkontakteScopeList.offers | VkontakteScopeList.pages | VkontakteScopeList.photos | VkontakteScopeList.questions | VkontakteScopeList.video | VkontakteScopeList.wall);
        public string Auth(string login, string pass)
        {
            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(String.Format("http://api.vk.com/oauth/authorize?client_id={0}&scope={1}&display=wap&v=5.27&response_type=token", appId, scope));
            HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
            StreamReader myStream = new StreamReader(myResp.GetResponseStream(), Encoding.UTF8);
            string html = myStream.ReadToEnd();
             

            Regex myReg = new Regex("<form(.*?)>(?<form_body>.*?)</form>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
            if (!myReg.IsMatch(html) || (html = myReg.Match(html).Groups["form_body"].Value) == "")
            {
                //MessageBox.Show("Не удалось получить форму авторизации. Проверьте шаблон регулярного выражения.");
                return "Error";
            }


            myReg = new Regex("<input(.*?)name=\"(?<name>[^\x22]+)\"(.*?)((value=\"(?<value>[^\x22]*)\"(.*?))|(.?))>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
            NameValueCollection qs = new NameValueCollection();
            foreach (Match m in myReg.Matches(html))
            {
                string val = m.Groups["value"].Value;
                if (m.Groups["name"].Value == "email")
                {
                    val = login;
                }
                else if (m.Groups["name"].Value == "pass")
                {
                    val = pass;
                }
                qs.Add(m.Groups["name"].Value, HttpUtility.UrlEncode(val));
            }

            byte[] b = System.Text.Encoding.UTF8.GetBytes(String.Join("&", from item in qs.AllKeys select item + "=" + qs[item]));

            myReq = (HttpWebRequest)HttpWebRequest.Create("https://login.vk.com/?act=login&soft=1&utf8=1");
            myReq.CookieContainer = new CookieContainer();
            myReq.Method = "POST";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.ContentLength = b.Length;
            myReq.GetRequestStream().Write(b, 0, b.Length);
            myReq.AllowAutoRedirect = false;
            myResp = (HttpWebResponse)myReq.GetResponse();
            CookieContainer cc = new CookieContainer();
            foreach (Cookie c in myResp.Cookies)
            {
                cc.Add(c);
            }

            if (!String.IsNullOrEmpty(myResp.Headers["Location"]))
            {
                // делаем редирект
                myReq = (HttpWebRequest)HttpWebRequest.Create(myResp.Headers["Location"]);
                myReq.CookieContainer = cc;// передаем куки
                myReq.Method = "GET";
                myReq.ContentType = "text/html";

                myResp = (HttpWebResponse)myReq.GetResponse();
                myStream = new StreamReader(myResp.GetResponseStream(), Encoding.UTF8);
                html = myStream.ReadToEnd();
            }
            else
            {
                // что-то пошло не так
                return "Error";
            }

            myReg = new Regex("<form(.*?)action=\"(?<post_url>[^\\x22]+)\"(.*?)>(?<form_body>.*?)</form>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
            if (!myReg.IsMatch(html))
            {
                return "error 3";
            }

            string url = myReg.Match(html).Groups["post_url"].Value;
            if (!url.ToLower().StartsWith("https://")) { url = String.Format("http://api.vkontakte.ru{0}", url); }

            myReq = (HttpWebRequest)HttpWebRequest.Create(url);
            myReq.CookieContainer = cc;
            myReq.Method = "POST";
            myReq.ContentLength = b.Length;
            myReq.GetRequestStream().Write(b, 0, b.Length);
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.AllowAutoRedirect = false;

            myResp = (HttpWebResponse)myReq.GetResponse();

            if (!String.IsNullOrEmpty(myResp.Headers["Location"]))
            {
                myReg = new Regex(@"(?<name>[\w\d\x5f]+)=(?<value>[^\x26\s]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in myReg.Matches(myResp.Headers["Location"]))
                {
                    if (m.Groups["name"].Value == "access_token")
                    {
                        accessToken = m.Groups["value"].Value;
                    }
                    else if (m.Groups["name"].Value == "user_id")
                    {
                        userId = m.Groups["value"].Value;
                    }
                    // еще можно запомнить срок жизни access_token - expires_in,
                    // если нужно
                }
            }
            else
            {
                return "error";
            }

           

            return accessToken;
        }
    }
}
