using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configuration;
using HttpServerLibrary.HttpResponse;
using Microsoft.Data.SqlClient;
using OrmLibrary;
using WebServer.Models;

namespace WebServer.Endpoints
{
    public class UserEndpoint : EndpointBase
    {
        [Get("user")]
        public IHttpResponseResult GetUserById(Guid id)
        {
            var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));

            var user = context.ReadById(id);
            return Json(user);
        }

        [Get("users")]
        public IHttpResponseResult GetAllUsers()
        {
            var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));

            var user = context.ReadAll();
            return Json(user);
        }

        [Get("users/delete")]
        public IHttpResponseResult DeleteUser(Guid id)
        {
            var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));

            var user = context.Delete(id);
            return Json(user);
        }

        [Post("users/create")]
        public IHttpResponseResult CreateUser(string login, string password)
        {
            var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));
            
            return Json(new { Success = context.Create(new User { Login = login, Password = password }) });
        }

        [Post("users/update")]
        public IHttpResponseResult UpdateUser(string login, string password, string newPassword)
        {
            var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));
            
            var user = context.FirstOrDefault(u => u.Login == login && u.Password == password);
            
            return Json(new { Success = context.Update(new User { Id = user.Id, Login = login, Password = newPassword }) });
        }
    }
}
