using System.Text;
using System.Text.Json;

namespace HttpServerLibrary.HttpResponse
{
    public class JsonResult : IHttpResponseResult
    {
        private readonly object _data;
        public JsonResult(object data)
        {
            _data = data;
        }

        public void Execute(HttpRequestContext context)
        {
            var json = JsonSerializer.Serialize(_data);

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            context.Response.Headers.Add("Content-Type", "application/json");
            // получаем поток ответа и пишем в него ответ
            context.Response.ContentLength64 = buffer.Length;
            using Stream output = context.Response.OutputStream;

            // отправляем данные
            output.Write(buffer);
            output.Flush();
        }
    }
}