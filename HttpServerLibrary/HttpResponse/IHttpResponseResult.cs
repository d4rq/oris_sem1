namespace HttpServerLibrary.HttpResponse
{
    public interface IHttpResponseResult
    {
        void Execute(HttpRequestContext context);
    }
}
