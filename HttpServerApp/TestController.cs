using HttpServerLib;

namespace HttpServerApp
{
    public class TestController
    {
        public object Hello(HttpRequestEntity request)
        {
            request.Response.Content = "Hello World!";

            return null;
        }

        public void Account(HttpRequestEntity request, string name, int age)
        {
            request.Response.Content = $"Welcome {name}! Your Age is {age}";
        }

        public object Post(HttpRequestEntity request)
        {
            request.Response.Content = "OK";

            return null;
        }
    }
}