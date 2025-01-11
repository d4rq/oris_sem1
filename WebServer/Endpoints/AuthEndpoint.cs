using System.Net;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configuration;
using HttpServerLibrary.HttpResponse;
using Microsoft.Data.SqlClient;
using OrmLibrary;
using WebServer.Models;
using WebServer.Repositories;
using WebServer.services;

namespace WebServer.Endpoints;

public class AuthEndpoint : EndpointBase
{
    [Get("auth/login")]
    public IHttpResponseResult GetLoginPage()
    {
        return Html(File.ReadAllText("./zonafilm/html/login.html"));
    }

    [Post("auth/login")]
    public IHttpResponseResult Login(string login, string password)
    {
        var repository = new UserRepository();
        
        var user = repository.GetUser(login, password);


        if(user == null)
        {
            return Redirect("/auth/login");
        }
            
        var token = Guid.NewGuid().ToString();
        Cookie nameCookie = new Cookie("session-token", token);
        nameCookie.Path = "/";

        Context.Response.Cookies.Add(nameCookie);
        SessionStorage.SaveSession(token, user.Id.ToString());
            
        Console.WriteLine($"Login: {login}, Password: {password}");
        return Redirect("/admin");
    }
        
    [Get("auth/register")]
    public IHttpResponseResult GetRegisterPage(string login, string password)
    {
        return Html(File.ReadAllText("./zonafilm/html/register.html"));
    }

    [Post("auth/register")]
    public IHttpResponseResult Register(string login, string password)
    {
        var repository = new UserRepository();

        repository.AddUser(new User() { Login = login, Password = password });
            
        return Redirect("/auth/login");
    }
}