using System.Net.Sockets;
using System.Threading.Tasks;

namespace HttpServerLib
{
    public class HttpRequestEntity
    {
        #region Headers

        /// <summary>
        /// CONNECT, DELETE, GET, HEAD, OPTIONS, PATCH, POST, PUT
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// HTTP/1.0
        /// HTTP/1.1
        /// </summary>
        public string Version { get; set; }
        public string Accept { get; set; }
        public string AcceptCharset { get; set; }
        public string AcceptEncoding { get; set; }
        public string AcceptLanguage { get; set; }
        public string Authorization { get; set; }
        public string CacheControl { get; set; }
        public string Connection { get; set; }
        public string Date { get; set; }
        public string Expect { get; set; }
        public string From { get; set; }
        public string Host { get; set; }
        public string IfMatch { get; set; }
        public string IfModifiedSince { get; set; }
        public string IfNoneMatch { get; set; }
        public string IfRange { get; set; }
        public string IfUnmodifiedSince { get; set; }
        public string UserAgent { get; set; }
        public string Via { get; set; }
        public string Warning { get; set; }

        #endregion Headers

        #region Content.Headers

        public string Allow { get; set; }
        public string ContentDisposition { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentLanguage { get; set; }
        public long ContentLength { get; set; }
        public string ContentLocation { get; set; }
        public string ContentMD5 { get; set; }
        public string ContentRange { get; set; }
        public string ContentType { get; set; }
        public string Expires { get; set; }
        public string LastModified { get; set; }

        #endregion Content.Headers

        public string Content { get; set; }
        public string QueryParameters { get; set; }

        public TcpClient Client { get; set; }
        public HttpResponseEntity Response { get; set; }
        public bool Handled { get; set; }

        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "Accept")
                {
                    return Accept;
                }
                else if (propertyName == "Accept-Charset")
                {
                    return AcceptCharset;
                }
                else if (propertyName == "Accept-Encoding")
                {
                    return AcceptEncoding;
                }
                else if (propertyName == "Accept-Language")
                {
                    return AcceptLanguage;
                }
                else if (propertyName == "Allow")
                {
                    return Allow;
                }
                else if (propertyName == "Authorization")
                {
                    return Authorization;
                }
                else if (propertyName == "Cache-Control")
                {
                    return CacheControl;
                }
                else if (propertyName == "Connection")
                {
                    return Connection;
                }
                else if (propertyName == "Content")
                {
                    return Content;
                }
                else if (propertyName == "Content-Disposition")
                {
                    return ContentDisposition;
                }
                else if (propertyName == "Content-Encoding")
                {
                    return ContentEncoding;
                }
                else if (propertyName == "Content-Language")
                {
                    return ContentLanguage;
                }
                else if (propertyName == "Content-Length")
                {
                    return ContentLength.ToString();
                }
                else if (propertyName == "Content-Location")
                {
                    return ContentLocation;
                }
                else if (propertyName == "Content-MD5")
                {
                    return ContentMD5;
                }
                else if (propertyName == "Content-Range")
                {
                    return ContentRange;
                }
                else if (propertyName == "Content-Type")
                {
                    return ContentType;
                }
                else if (propertyName == "Date")
                {
                    return Date;
                }
                else if (propertyName == "Expect")
                {
                    return Expect;
                }
                else if (propertyName == "Expires")
                {
                    return Expires;
                }
                else if (propertyName == "From")
                {
                    return From;
                }
                else if (propertyName == "Host")
                {
                    return Host;
                }
                else if (propertyName == "If-Match")
                {
                    return IfMatch;
                }
                else if (propertyName == "If-Modified-Since")
                {
                    return IfModifiedSince;
                }
                else if (propertyName == "If-None-Match")
                {
                    return IfNoneMatch;
                }
                else if (propertyName == "If-Range")
                {
                    return IfRange;
                }
                else if (propertyName == "If-Unmodified-Since")
                {
                    return IfUnmodifiedSince;
                }
                else if (propertyName == "Last-Modified")
                {
                    return LastModified;
                }
                else if (propertyName == "Method")
                {
                    return Method;
                }
                else if (propertyName == "User-Agent")
                {
                    return UserAgent;
                }
                else if (propertyName == "Version")
                {
                    return Version;
                }
                else if (propertyName == "Via")
                {
                    return Via;
                }
                else if (propertyName == "Warning")
                {
                    return Warning;
                }

                return null;
            }
            set
            {
                if (propertyName == "Accept")
                {
                    Accept = value;
                }
                else if (propertyName == "Accept-Charset")
                {
                    AcceptCharset = value;
                }
                else if (propertyName == "Accept-Encoding")
                {
                    AcceptEncoding = value;
                }
                else if (propertyName == "Accept-Language")
                {
                    AcceptLanguage = value;
                }
                else if (propertyName == "Allow")
                {
                    Allow = value;
                }
                else if (propertyName == "Authorization")
                {
                    Authorization = value;
                }
                else if (propertyName == "Cache-Control")
                {
                    CacheControl = value;
                }
                else if (propertyName == "Connection")
                {
                    Connection = value;
                }
                else if (propertyName == "Content")
                {
                    Content = value;
                }
                else if (propertyName == "Content-Disposition")
                {
                    ContentDisposition = value;
                }
                else if (propertyName == "Content-Encoding")
                {
                    ContentEncoding = value;
                }
                else if (propertyName == "Content-Language")
                {
                    ContentLanguage = value;
                }
                else if (propertyName == "Content-Length")
                {
                    long contentLength;
                    if (long.TryParse(value, out contentLength))
                        ContentLength = contentLength;
                }
                else if (propertyName == "Content-Location")
                {
                    ContentLocation = value;
                }
                else if (propertyName == "Content-MD5")
                {
                    ContentMD5 = value;
                }
                else if (propertyName == "Content-Range")
                {
                    ContentRange = value;
                }
                else if (propertyName == "Content-Type")
                {
                    ContentType = value;
                }
                else if (propertyName == "Date")
                {
                    Date = value;
                }
                else if (propertyName == "Expect")
                {
                    Expect = value;
                }
                else if (propertyName == "Expires")
                {
                    Expires = value;
                }
                else if (propertyName == "From")
                {
                    From = value;
                }
                else if (propertyName == "Host")
                {
                    Host = value;
                }
                else if (propertyName == "If-Match")
                {
                    IfMatch = value;
                }
                else if (propertyName == "If-Modified-Since")
                {
                    IfModifiedSince = value;
                }
                else if (propertyName == "If-None-Match")
                {
                    IfNoneMatch = value;
                }
                else if (propertyName == "If-Range")
                {
                    IfRange = value;
                }
                else if (propertyName == "If-Unmodified-Since")
                {
                    IfUnmodifiedSince = value;
                }
                else if (propertyName == "Last-Modified")
                {
                    LastModified = value;
                }
                else if (propertyName == "Method")
                {
                    Method = value;
                }
                else if (propertyName == "User-Agent")
                {
                    UserAgent = value;
                }
                else if (propertyName == "Version")
                {
                    Version = value;
                }
                else if (propertyName == "Via")
                {
                    Via = value;
                }
                else if (propertyName == "Warning")
                {
                    Warning = value;
                }
            }
        }

        public override string ToString()
        {
            return $@"Method: {Method}
Version: {Version}
Host: {Host}
QueryParams: {QueryParameters}
ContentType: {ContentType}
UserAgent: {UserAgent}
";
        }
    }
}