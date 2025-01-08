using HttpServerLibrary;
using HttpServerLibrary.Attributes;

namespace WebServer.Endpoints
{
    internal class TestEndpoint : EndpointBase
    {
        [Get("test")]
        public void Wow()
        {
            Console.WriteLine("hello");
        }
    }
}
