using HttpServerLib;
using System;
using System.Collections.Generic;
using System.IO;
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

        static async Task MainAsync(params string[] args)
        {
            string ip = "127.0.0.1";

            string portNumber = "8080";

            int receiveBufferSize = 8 * 1024;//8kb

            int backlogClientCount = 3000;

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

            using (SimpleHttpServer httpServer = new SimpleHttpServer(ip, portNumber, receiveBufferSize, backlogClientCount, sourceDir, cts.Token) { GetRoutes = getRoutes, PostRoutes = postRoutes })
            {
                Console.WriteLine("Simple HTTP Server running...");
                Console.WriteLine("=============================");

                //Start default web browser
                //Helper.OpenBrowser($"http://{ip}:{portNumber}");
                //Helper.OpenBrowser($"http://{ip}:{portNumber}/hello");
                Helper.OpenBrowser($"http://{ip}:{portNumber}/account/Kenan/33");

                await httpServer.RequestReceivedAsync(async httpRequest =>
                {
                    string queryParams = httpRequest.QueryParameters.ToLowerInvariant();
                    int parameterStartIndex = queryParams.IndexOf('/', 1);
                    string actionName = queryParams;
                    string[] paramArray = new string[0];

                    if (parameterStartIndex > -1)
                    {
                        actionName = queryParams.Substring(0, parameterStartIndex);
                        string parameters = queryParams.Substring(parameterStartIndex);
                        paramArray = parameters.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    if (httpRequest.Method == "GET")
                    {
                        if (queryParams == "/" || Path.HasExtension(queryParams))
                        {
                            string fileName = httpRequest.QueryParameters.Substring(1);

                            if (String.IsNullOrEmpty(fileName))
                                fileName = "iisstart.htm";

                            string filePath = Path.Combine(httpServer.SourceDirectory, fileName);

                            await HttpUtility.UploadFile(filePath, httpRequest);

                            httpRequest.Handled = true;
                        }
                        else
                        {
                            RouteInfo routeInfo;
                            if (getRoutes.TryGetValue(actionName, out routeInfo))
                            {
                                object[] actionParameters;
                                if (routeInfo.ControllerInstance == null)
                                {
                                    actionParameters = new object[paramArray.Length + 1];
                                    actionParameters[0] = httpRequest;
                                    paramArray.CopyTo(actionParameters, 1);
                                }
                                else
                                {
                                    actionParameters = new object[paramArray.Length + 2];
                                    actionParameters[0] = routeInfo.ControllerInstance;
                                    actionParameters[1] = httpRequest;
                                    paramArray.CopyTo(actionParameters, 2);
                                }

                                object result = routeInfo.ActionInvoker(actionParameters);
                            }
                        }
                    }
                    else if (httpRequest.Method == "POST")
                    {
                        RouteInfo routeInfo;
                        if (postRoutes.TryGetValue(actionName, out routeInfo))
                        {
                            object[] actionParameters;
                            if (routeInfo.ControllerInstance == null)
                            {
                                actionParameters = new object[paramArray.Length + 1];
                                actionParameters[0] = httpRequest;
                                paramArray.CopyTo(actionParameters, 1);
                            }
                            else
                            {
                                actionParameters = new object[paramArray.Length + 2];
                                actionParameters[0] = routeInfo.ControllerInstance;
                                actionParameters[1] = httpRequest;
                                paramArray.CopyTo(actionParameters, 2);
                            }

                            object result = routeInfo.ActionInvoker(actionParameters);
                        }
                    }

                    //Console.WriteLine(httpRequest);
                });
            }
        }
    }
}