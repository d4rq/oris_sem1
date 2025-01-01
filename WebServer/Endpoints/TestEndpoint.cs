using HttpServerLibrary;
using HttpServerLibrary.Attributes;

namespace WebServer.Endpoints
{
    internal class TestEndpoint : EndpointBase
    {
        [Get("wow")]
        public void Wow(string hello)
        {
            Console.WriteLine(hello);
        }
    }
}
