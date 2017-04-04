using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServerLib
{
    public class SimpleHttpServer : IDisposable
    {
        #region Properties

        public string Ip { get; private set; }
        public string PortNumber { get; private set; }
        public int ReceiveBufferSize { get; private set; }
        public int BacklogClientCount { get; private set; }
        public string SourceDirectory { get; private set; }
        public CancellationToken Token { get; private set; }
        public virtual int ReceivedRequestCount => receivedRequestCount;
        public Dictionary<string, RouteInfo> GetRoutes { get; set; } = new Dictionary<string, RouteInfo>();
        public Dictionary<string, RouteInfo> PostRoutes { get; set; } = new Dictionary<string, RouteInfo>();

        protected TcpListener tcpListener;
        int receivedRequestCount = 0;
        MemoryStream favicon;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip">127.0.0.1</param>
        /// <param name="portNumber">8080</param>
        /// <param name="receiveBufferSize">1024 * 8 (8kb)</param>
        /// <param name="backlogClientCount">1000</param>
        /// <param name="token"></param>
        public SimpleHttpServer(string ip, string portNumber, int receiveBufferSize, int backlogClientCount, string sourceDirectory, CancellationToken token)
        {
            Ip = ip;
            PortNumber = portNumber;
            ReceiveBufferSize = receiveBufferSize == 0 ? (8 * 1024) : receiveBufferSize;
            BacklogClientCount = backlogClientCount;
            SourceDirectory = sourceDirectory;
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!isWindows)
                SourceDirectory = SourceDirectory.Replace('\\', Path.DirectorySeparatorChar);
            SourceDirectory = sourceDirectory;
            Token = token;

            IPAddress ipAddress;

            if (!IPAddress.TryParse(ip, out ipAddress))
                throw new ArgumentOutOfRangeException("ip");

            int port;

            if (!int.TryParse(portNumber, out port) || port < 1024 || port > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException("port");

            string faviconPath = Path.Combine(SourceDirectory, "favicon.png");

            favicon = new MemoryStream();

            if (File.Exists(faviconPath))
                using (FileStream fileStream = File.OpenRead(faviconPath))
                    fileStream.CopyTo(favicon);

            tcpListener = new TcpListener(ipAddress, port);

            tcpListener.Start(BacklogClientCount);
        }

        #endregion Constructor

        public async Task RequestReceivedAsync(Func<HttpRequestEntity, Task> callback = null)
        {
            try
            {
                byte[] responseBuffer;

                while (true)
                {
                    try
                    {
                        using (TcpClient acceptedTcpClient = await tcpListener.AcceptTcpClientAsync().ContinueWith(t => t.Result, Token))
                        {
                            using (NetworkStream networkStream = acceptedTcpClient.GetStream())
                            {
                                Interlocked.Increment(ref receivedRequestCount);

                                HttpRequestEntity httpRequest = await HttpUtility.ParseHttpRequest(networkStream, acceptedTcpClient);

                                if (httpRequest == null) continue;

                                #region Controller

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


                                if (queryParams == "/" || Path.HasExtension(queryParams))
                                {
                                    string fileName = httpRequest.QueryParameters.Substring(1);

                                    if (String.IsNullOrEmpty(fileName))
                                        fileName = "iisstart.htm";

                                    if (fileName == "favicon.ico")
                                    {
                                        await HttpUtility.UploadStream(favicon, httpRequest);
                                    }
                                    else
                                    {
                                        string filePath = Path.Combine(SourceDirectory, fileName);

                                        await HttpUtility.UploadFile(filePath, httpRequest);
                                    }

                                    httpRequest.Handled = true;
                                }
                                else
                                {
                                    Dictionary<string, RouteInfo> routeDict = GetRoutes;

                                    if (httpRequest.Method == "POST")
                                        routeDict = PostRoutes;

                                    RouteInfo routeInfo;
                                    if (routeDict.TryGetValue(actionName, out routeInfo))
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

                                #endregion Controller

                                if (callback != null)
                                    await callback(httpRequest);

                                if (!httpRequest.Handled)
                                {
                                    responseBuffer = HttpUtility.BuildHttpResponse(httpRequest);

                                    await networkStream.WriteAsync(responseBuffer, 0, responseBuffer.Length, Token);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync("SERVER ------" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("SERVER ------" + ex.ToString());
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TcpListener tcpListener_ = tcpListener;
                    tcpListener = null;
                    if (tcpListener_ != null)
                    {
                        tcpListener_.Stop();
                        tcpListener_ = null;
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}