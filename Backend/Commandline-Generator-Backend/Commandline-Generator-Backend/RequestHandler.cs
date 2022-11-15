using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Commandline_Generator_Backend
{
    public class Test
    {
        public int test { get; set; }
    }

    public class Response
    {
        public Response(int intCode, object obj)
        {
            code = intCode;
            result = intCode == 0 ? "ack" : "nack";
            data = obj;
        }
        public string result { get; set; }
        public int code { get; set; }
        public object data { get; set; }
    }

    internal class RequestHandler
    {
        public static void HandleRequest(object obj)
        {
            HttpListenerContext ctx = obj as HttpListenerContext;
            HttpListenerRequest req = ctx.Request;
            HttpListenerResponse resp = ctx.Response;

            if (!((req.HttpMethod == "GET" || req.HttpMethod == "POST") && req.HasEntityBody))
            {
                sendResponse(1, null, resp);
                return;
            }

            Stream body = req.InputStream;
            Encoding encoding = req.ContentEncoding;
            StreamReader reader = new System.IO.StreamReader(body, encoding);
            string s = reader.ReadToEnd();
            Test? test = JsonSerializer.Deserialize<Test>(s);

            Console.WriteLine(req.HttpMethod + " " + req.Url.ToString() + " " + req.UserHostName + " " + req.UserAgent);

            sendResponse(0, null, resp);
        }

        private static async void sendResponse(int code, object obj, HttpListenerResponse resp)
        {
            Response respData = new Response(code, obj);
            string result = JsonSerializer.Serialize(respData);
            byte[] data = Encoding.UTF8.GetBytes(result);
            resp.ContentType = "text/json";
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }
    }
}
