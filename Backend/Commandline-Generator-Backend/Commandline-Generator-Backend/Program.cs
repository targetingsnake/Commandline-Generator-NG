// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using System.Reflection.Metadata.Ecma335;

namespace Commandline_Generator_Backend
{
    class HttpServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";

        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                Thread Worker = new Thread(RequestHandler.HandleRequest);
                Worker.Start(ctx);
            }
        }


        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}
