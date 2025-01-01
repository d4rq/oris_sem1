using System.Net;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponse;
using OrmLibrary;
using Microsoft.Data.SqlClient;
using HttpServerLibrary.Configuration;
using WebServer.Models;
using WebServer.services;

namespace WebServer.Endpoints
{
    internal class AuthEndpoint : EndpointBase
    {
        [Get("auth/login")]
        public IHttpResponseResult GetLoginPage()
        {
            return Html(File.ReadAllText("Templates/Pages/Auth/signin.html"));
        }

        [Post("auth/login")]
        public IHttpResponseResult Login(string login, string password)
        {
            var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));

            var user = context.FirstOrDefault(u => u.Login == login && u.Password == password);

            if(user == null)
            {
                var loginPage = File.ReadAllText(@"Templates\Pages\Dashboard\index.html");               
                return Html(loginPage);
            }
            
            var token = Guid.NewGuid().ToString();
            Cookie nameCookie = new Cookie("session-token", token);
            nameCookie.Path = "/";

            Context.Response.Cookies.Add(nameCookie);
            SessionStorage.SaveSession(token, user.Id.ToString());
            
            Console.WriteLine($"Login: {login}, Password: {password}");
            return Redirect("/dashboard");
        }
        
        [Get("auth/register")]
        public IHttpResponseResult GetRegisterPage(string login, string password)
        {
            return Html(File.ReadAllText("Templates/Pages/Auth/signup.html"));
        }

        [Post("auth/register")]
        public IHttpResponseResult Register(string login, string password)
        {
            var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));

            context.Create(new User { Login = login, Password = password });
            
            return Redirect("/dashboard");
        }
    }
}
