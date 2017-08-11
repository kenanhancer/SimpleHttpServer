using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerLib
{
    public static class HttpUtility
    {
        public static IDictionary<string, string> MimeTypeDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
    };
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
            int bytesRead;
            byte[] responseBuffer = new byte[8192];

            stream.Position = 0;

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