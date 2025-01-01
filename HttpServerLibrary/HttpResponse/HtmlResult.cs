using System.Text;

namespace HttpServerLibrary.HttpResponse
{
    public class HtmlResult : IHttpResponseResult
    {
        private readonly string _html;
        public HtmlResult(string html)
        {
            _html = html;
        }

        public void Execute(HttpRequestContext context)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(_html);

            // получаем поток ответа и пишем в него ответ
            context.Response.ContentLength64 = buffer.Length;
            using Stream output = context.Response.OutputStream;

            // отправляем данные
            output.Write(buffer);
            output.Flush();
        }
    }
}
