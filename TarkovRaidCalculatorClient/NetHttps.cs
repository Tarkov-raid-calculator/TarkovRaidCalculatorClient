using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TarkovRaidCalculatorClient.Dto;

namespace TarkovRaidCalculatorClient
{
    class NetHttps : INet
    {
        public async Task<Auth> AuthAsync(string email, string hash)
        {
            try
            {
                string jsonStr = GetRes(await SendReqJSONAsync("http://192.168.2.37:10000/auth/login/", Methode.POST, JsonConvert.SerializeObject(new Dto.AuthIn { Email = email, PasswordHash = hash }), "application/json"));
                Dto.AuthOut authOut = JsonConvert.DeserializeObject<Dto.AuthOut>(jsonStr);
                return new Auth { Token = authOut.Token, RefreshToken = authOut.RefreshToken };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Auth Auth(string email, string hash)
        {
            try
            {
                string str = JsonConvert.SerializeObject(new Dto.AuthIn { Email = email, PasswordHash = hash });
                string jsonStr = GetRes(SendReqJSON("http://192.168.2.37:10000/auth/login/", Methode.POST, JsonConvert.SerializeObject(new Dto.AuthIn { Email = email, PasswordHash = hash }), "application/json"));
                Dto.AuthOut authOut = JsonConvert.DeserializeObject<Dto.AuthOut>(jsonStr);
                return new Auth { Token = authOut.Token, RefreshToken = authOut.RefreshToken };
            }
            catch (Exception e)
            {
                return null;
            }

        }

        private WebResponse Test(string url, Methode methode = Methode.GET, string body = "", string contentType = "")
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = getMethodeString(methode);
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            webRequest.ContentType = contentType;
            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {

                streamWriter.Write(body);
                streamWriter.Flush();
            }
            return webRequest.GetResponse();
        }
        private WebResponse SendReqJSON(string url, Methode methode = Methode.GET, string body = "", string contentType = "")
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = getMethodeString(methode);
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            if (getMethodeString(methode) != "GET")
            {
                webRequest.ContentType = contentType;
                using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                {

                    streamWriter.Write(body);
                    streamWriter.Flush();
                }
            }
            try
            {
                return webRequest.GetResponse();
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private async Task<WebResponse> SendReqJSONAsync(string url, Methode methode = Methode.GET, string body = "", string contentType = "", string bearerToken = "")
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = getMethodeString(methode);
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            if (bearerToken != "")
            {
                webRequest.PreAuthenticate = true;
                webRequest.Headers.Add("Authorization", "Bearer " + bearerToken);
            }
            if (getMethodeString(methode) != "GET")
            {
                webRequest.ContentType = contentType;
                using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                {

                    streamWriter.Write(body);
                    streamWriter.Flush();
                }
            }
            try
            {
                return await webRequest.GetResponseAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private string GetRes(WebResponse webResponse)
        {
            Stream resStream = webResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(resStream, encode);
            string res = "";
            Char[] read = new Char[256];

            // Read 256 charcters at a time.    
            int count = readStream.Read(read, 0, 256);

            while (count > 0)
            {
                // Dump the 256 characters on a string and display the string onto the console.
                String str = new String(read, 0, count);
                res += new String(read, 0, count);
                count = readStream.Read(read, 0, 256);
            }
            readStream.Close();
            webResponse.Close();
            return res;
        }


        enum Methode
        {
            GET,
            HEAD,
            POST,
            PUT,
            DELETE,
            CONNECT,
            OPTIONS,
            TRACE,
            PATCH
        }
        private string getMethodeString(Methode methode)
        {
            switch (methode)
            {
                case Methode.GET:
                    return "GET";
                case Methode.HEAD:
                    return "HEAD";
                case Methode.POST:
                    return "POST";
                case Methode.PUT:
                    return "PUT";
                case Methode.DELETE:
                    return "DELETE";
                case Methode.CONNECT:
                    return "CONNECT";
                case Methode.OPTIONS:
                    return "OPTIONS";
                case Methode.TRACE:
                    return "TRACE";
                case Methode.PATCH:
                    return "PATCH";
                default:
                    return "";
            }
        }

        public async Task<RefreshAuthOut> RefreshTokenAsync(string token, string refreshToken)
        {
            string jsonStr = GetRes(await SendReqJSONAsync("http://192.168.2.37:10000/auth/refreshToken/", Methode.POST, JsonConvert.SerializeObject(new Dto.RefreshAuthIn { RefreshToken = refreshToken }), "application/json", token));
            return JsonConvert.DeserializeObject<Dto.RefreshAuthOut>(jsonStr);
        }
        private static string CreateFormDataBoundary()
        {
            return "---------------------------" + DateTime.Now.Ticks.ToString("x");
        }
        public async Task SendImage(byte[] imageData, string token, string fileName, string raid = "pre")
        {

            //ExecutePostRequest("http://192.168.2.37:9000/file/", new Dictionary<string, string>(), ,".jpg");
            
            try
            {
                string requestURL = "http://192.168.2.37:9000/file";
                //WebClient wc = new WebClient();
                //byte[] bytes = wc.DownloadData(fileName); // You need to do this download if your file is on any other server otherwise you can convert that file directly to bytes  
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                // Add your parameters here  
                postParameters.Add("fileToUpload", new FormUpload.FileParameter(imageData, Path.GetFileName(fileName), "image/png"));
                string userAgent = "Someone";
                HttpWebResponse webResponse = FormUpload.MultipartFormPost(requestURL, userAgent, postParameters, "raid", raid, token);
                // Process response  
                StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                //return ResponseText = responseReader.ReadToEnd();
                webResponse.Close();
            }
            catch (Exception exp) {
            }
            
            //string boundary = "------------------------" + DateTime.Now.Ticks;
            //await SendReqImageAsync("http://192.168.2.37:9000/file/", imageData, Methode.POST, "multipart/form-data; boundary=" + boundary, token, boundary);
        }
        private async Task<WebResponse> SendReqImageAsync(string url, byte[] body, Methode methode = Methode.GET, string contentType = "", string bearerToken = "", string boundary ="")
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = getMethodeString(methode);
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            if (bearerToken != "")
            {
                webRequest.PreAuthenticate = true;
                webRequest.Headers.Add("Authorization", "Bearer " + bearerToken);
            }
            if (getMethodeString(methode) != "GET")
            {
                webRequest.ContentType = contentType;
                webRequest.ContentLength = body.Length;
                /*
                {
                    var reqWriter = new StreamWriter(reqStream);
                    var tmp = string.Format(propFormat, "str1", "hello world");
                    reqWriter.Write(tmp);
                    //tmp = string.Format(propFormat, "str2", "hello world 2");
                    //reqWriter.Write(tmp);
                    reqWriter.Write("--" + boundary + "--");
                    reqWriter.Flush();
                }
                */
               /*
                Stream newStream = webRequest.GetRequestStream();
                newStream.Write(body, 0, body.Length);
                newStream.Close();
               */
            }
            try
            {
                return await webRequest.GetResponseAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
