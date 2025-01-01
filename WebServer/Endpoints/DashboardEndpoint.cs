using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponse;
using HttpServerLibrary.Configuration;
using Microsoft.Data.SqlClient;
using OrmLibrary;
using WebServer.Models;
using WebServer.services;

namespace WebServer.Endpoints
{
    internal class DashboardEndpoint : EndpointBase
    {
        [Get("dashboard")]
        public IHttpResponseResult GetPage()
        {
            if (!IsAuthorized(Context)) // Используем метод проверки авторизации
            {
                return Redirect("/auth/login");
            }
 
            var html= File.ReadAllText("Templates/Pages/Dashboard/index.html");
            var engine = new TemplateEngine.HtmlTemplateEngine();
            var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));
            var id = Guid.Parse((ReadOnlySpan<char>)SessionStorage.GetUserId(Context.Request.Cookies["session-token"]!.Value));
            var user = context.FirstOrDefault(u => u.Id == id);
            return Html(engine.Render(html, user));
        }
        
        private bool IsAuthorized(HttpRequestContext context)
        {
            // Проверка наличия Cookie с session-token
            //var cookies = context.Request.Cookies.First();
            if (context.Request.Cookies.Any(c=> c.Name == "session-token"))
            {
                var cookie = context.Request.Cookies["session-token"];
                return SessionStorage.ValidateToken(cookie.Value);
            }
         
            return false;
        }
    }
}
