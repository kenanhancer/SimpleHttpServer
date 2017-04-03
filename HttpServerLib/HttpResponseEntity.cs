using System.Net;
using System.Net.Sockets;

namespace HttpServerLib
{
    public class HttpResponseEntity
    {
        public string Version { get; set; }
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Status { get; set; }
        public NetworkStream ResponseStream { get; set; }

        public string AcceptRanges { get; set; }
        public string Age { get; set; }
        public string CacheControl { get; set; }
        public string Connection { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public string Vary { get; set; }
        public string Warning { get; set; }
    }
}