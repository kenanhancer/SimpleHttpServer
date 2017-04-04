using HttpServerLib;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServerApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(params string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;

                cts.Cancel();
            };

            string sourceDir = @"http\";

            TestController controller = new TestController();

            var getRoutes = new Dictionary<string, RouteInfo>();
            getRoutes.Add("/hello", new RouteInfo { ActionName = "Hello", ControllerType = typeof(Program) });
            getRoutes.Add("/account", new RouteInfo { ActionName = "Account", ControllerInstance = controller });

            var postRoutes = new Dictionary<string, RouteInfo>();
            postRoutes.Add("/", new RouteInfo { ActionName = "Post", ControllerType = typeof(Program) });

            using (SimpleHttpServer httpServer = new SimpleHttpServer("127.0.0.1", "8080", (8 * 1024), 3000, sourceDir, cts.Token) { GetRoutes = getRoutes, PostRoutes = postRoutes })
            {
                Console.WriteLine("Simple HTTP Server Running...");
                Console.WriteLine("=============================");


                Helper.OpenBrowser($"http://{httpServer.Ip}:{httpServer.PortNumber}");
                Helper.OpenBrowser($"http://{httpServer.Ip}:{httpServer.PortNumber}/hello");
                Helper.OpenBrowser($"http://{httpServer.Ip}:{httpServer.PortNumber}/account/Kenan/33");


                await httpServer.RequestReceivedAsync(async httpRequest =>
                {
                    await Console.Out.WriteLineAsync(httpRequest.ToString());
                });
            }
        }

        public static object Hello(HttpRequestEntity request)
        {
            request.Response.Content = "Hello World!";

            return null;
        }

        public static void Account(HttpRequestEntity request, string name, int age)
        {
            request.Response.Content = $"Welcome {name}! Your Age is {age}";
        }

        public static object Post(HttpRequestEntity request)
        {
            request.Response.Content = "OK";

            return null;
        }
    }
}