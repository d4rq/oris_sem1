namespace HttpServerLibrary.HttpResponse;

public class RedirectResult: IHttpResponseResult
{
    private readonly string _location;
    public RedirectResult(string location)
    {
        _location = location;
    }
 
    public void Execute(HttpRequestContext context)
    {
        var response = context.Response;
        //response.StatusCode = 302;
        //response.RedirectLocation = _location;
        //response.Headers.Add("Location", _location); // Заголовок для указания пути
        response.Cookies.Add(context.Request.Cookies);
        response.Redirect(_location);
        response.Close();
    }
}