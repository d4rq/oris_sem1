using HttpServerLibrary.Configuration;
using Microsoft.Data.SqlClient;
using OrmLibrary;
using WebServer.Models;

namespace WebServer.Repositories;

public class UserRepository
{
    private readonly OrmContext<User> _context =
        new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));

    public User GetUser(string login, string password)
    {
        return _context.FirstOrDefault(u => u.Login == login && u.Password == password);
    }

    public bool AddUser(User user)
    {
        return _context.Create(user);
    }
    
    public User GetUserById(int id)
    {
        return _context.FirstOrDefault(u => u.Id == id);
    }
}