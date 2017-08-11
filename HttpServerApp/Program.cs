using HttpServerLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
//using UniCorn.IoC;

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

            List<RouteParser> routerList = new List<RouteParser>();
            routerList.Add(new RouteParser("/{controller}/{action}/"));
            routerList.Add(new RouteParser("/{controller}/{action}/{id}"));

            string url = "https://localhost:8080/Test/Index/";
            Uri uri = new Uri(url);

            //UniCorn.IoC.UniIoC container = new UniCorn.IoC.UniIoC();

            foreach (var router in routerList)
            {
                var _values = router.ParseRouteInstance(uri.PathAndQuery);

                string controllerName;
                if (_values.TryGetValue("controller", out controllerName))
                {
                    string _namespace = Assembly.GetEntryAssembly().GetName().Name;
                    Type _type = Type.GetType($"{_namespace}.{controllerName}Controller");

                    //if (container.IsRegistered(controllerName))
                    //    container.Register(ServiceCriteria.For(_type).Named(controllerName));

                    //object ooo = container.Resolve(controllerName);
                }
            }

            var parser = new RouteParser("https://localhost:8080/{controller}/{action}/");
            var values = parser.ParseRouteInstance("https://localhost:8080/Home/Index/");

            var parser2 = new RouteParser("https://localhost:8080/api/machine/{code}/all");
            var values2 = parser2.ParseRouteInstance("https://localhost:8080/api/machine/Home/All");


            var getRoutes = new Dictionary<string, RouteInfo>();
            getRoutes.Add("/hello", new RouteInfo { Controller = "Home", Action = "Hello", ControllerType = typeof(Program) });
            getRoutes.Add("/account", new RouteInfo { Controller = "Home", Action = "Account", ControllerInstance = controller });

            var postRoutes = new Dictionary<string, RouteInfo>();
            postRoutes.Add("/", new RouteInfo { Controller = "Home", Action = "Post", ControllerType = typeof(Program) });

            var routes = new Dictionary<string, string>();



            using (SimpleHttpServer httpServer = new SimpleHttpServer("127.0.0.1", "8080", (8 * 1024), 3000, sourceDir, cts.Token) { GetRoutes = getRoutes, PostRoutes = postRoutes })
            {
                Console.WriteLine("Simple HTTP Server Running...");
                Console.WriteLine("=============================");


                Helper.OpenBrowser($"http://{httpServer.Ip}:{httpServer.PortNumber}");
                Helper.OpenBrowser($"http://{httpServer.Ip}:{httpServer.PortNumber}/hello");
                Helper.OpenBrowser($"http://{httpServer.Ip}:{httpServer.PortNumber}/account/Kenan/33");


                await httpServer.RequestReceivedAsync(async httpRequest =>
                {
                    //await Console.Out.WriteLineAsync(httpRequest.ToString());
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