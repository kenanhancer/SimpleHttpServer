using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerLib
{
    public static class HttpUtility
    {
        public static byte[] BuildHttpResponse(HttpRequestEntity httpRequest)
        {
            StringBuilder sb = new StringBuilder();

            long contentLength = httpRequest.Response.ContentLength;
            string content = httpRequest.Response.Content;

            if (contentLength <= 0 && content != null)
                contentLength = content.Length;

            sb.AppendLine($"{httpRequest.Version} {(long)httpRequest.Response.StatusCode} {httpRequest.Response.Status}");
            sb.AppendLine($"Content-Type: {httpRequest.Response.ContentType}");
            sb.AppendLine($"Content-Length: {contentLength}");
            sb.AppendLine("Accept-Ranges: bytes");
            sb.AppendLine();
            if (!String.IsNullOrEmpty(content))
                sb.AppendLine(content);

            byte[] responseBuffer = Encoding.UTF8.GetBytes(sb.ToString());

            return responseBuffer;
        }

        public static async Task UploadFile(string filePath, HttpRequestEntity httpRequest)
        {
            if (File.Exists(filePath))
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    httpRequest.Response.ContentLength = fileStream.Length;

                    byte[] headerBuffer = BuildHttpResponse(httpRequest);

                    await httpRequest.Response.ResponseStream.WriteAsync(headerBuffer, 0, headerBuffer.Length);

                    int bytesRead;
                    byte[] responseBuffer = new byte[8192];

                    while ((bytesRead = await fileStream.ReadAsync(responseBuffer, 0, responseBuffer.Length)) != 0)
                    {
                        await httpRequest.Response.ResponseStream.WriteAsync(responseBuffer, 0, bytesRead);
                    }
                }
        }

        public static async Task UploadStream(Stream stream, HttpRequestEntity httpRequest)
        {
            httpRequest.Response.ContentLength = stream.Length;

            byte[] headerBuffer = BuildHttpResponse(httpRequest);

            await httpRequest.Response.ResponseStream.WriteAsync(headerBuffer, 0, headerBuffer.Length);

            int bytesRead;
            byte[] responseBuffer = new byte[8192];

            while ((bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length)) != 0)
            {
                await httpRequest.Response.ResponseStream.WriteAsync(responseBuffer, 0, bytesRead);
            }
        }

        public static async Task<HttpRequestEntity> ParseHttpRequest(NetworkStream networkStream, TcpClient tcpClient)
        {
            HttpRequestEntity requestMessage = null;

            StreamReader streamReader = new StreamReader(networkStream);

            try
            {
                string line = String.Empty;
                string[] parts;
                string headerName;
                string headerValue;
                string content = String.Empty;

                do
                {
                    line = await streamReader.ReadLineAsync();

                    if (line == null) continue;

                    if (line == "")
                    {
                        while (streamReader.Peek() != -1)
                        {
                            content += await streamReader.ReadLineAsync();
                        }
                        requestMessage.Content = content;
                        break;
                    }

                    parts = line.Split(new[] { ':' }, 2);

                    if (parts.Length == 0) continue;

                    if (parts.Length > 1)
                    {
                        headerName = parts[0];

                        headerValue = parts[1].Trim();

                        requestMessage[headerName] = headerValue;
                    }
                    else
                    {
                        requestMessage = new HttpRequestEntity { Client = tcpClient };

                        parts = line.Split(' ');

                        requestMessage.Method = parts[0];

                        requestMessage.Version = parts[2];

                        requestMessage.QueryParameters = parts[1];
                    }

                } while (streamReader.Peek() != -1);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }

            if (requestMessage != null)
                requestMessage.Response = new HttpResponseEntity { ResponseStream = networkStream, ContentType = requestMessage.ContentType, StatusCode = HttpStatusCode.OK, Status = "OK" };

            return requestMessage;
        }
    }
}