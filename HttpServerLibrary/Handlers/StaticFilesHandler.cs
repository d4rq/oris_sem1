using System.Net;
using System.Text;
using HttpServerLibrary.Configuration;

namespace HttpServerLibrary.Handlers
{
    internal sealed class StaticFilesHandler : Handler
    {
        public override void HandleRequest(HttpRequestContext context)
        {
            var request = context.Request; // Данные о запросе
            bool IsGet = request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase); // Проверка запроса на метод GET
            var absolutePath = request.Url!.AbsolutePath == "/" ? "html/index.html" : request.Url.AbsolutePath.Remove(0, 1); // Перенаправление с / на /html/index.html
            string[] arr = absolutePath.Split("."); // Вспомогательный массив для получения сведений о пути запроса
            bool IsFile = arr.Length >= 2; // Проверка на то, запрашивает ли клиент файл
            var config = AppConfig.GetInstance(); // Вытаскиваем конфиг

            if (IsGet && IsFile) // Проверка на GET и запрос файла
            {
                // TODO: разобраться с try-catch
                try
                {
                    string filePath = config.StaticDirectoryPath + absolutePath; // Получение пути к файлу

                    if (!File.Exists(filePath)) // Проверка существования файла
                    {
                        // Логика отпрвки Not Found в случае, если файл не существует
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound; // Статус код 404
                        var responseText = Encoding.UTF8.GetBytes("Not Found"); // Текст "не найдено"
                        context.Response.ContentEncoding = Encoding.UTF8; // Кодировка ответа
                        context.Response.ContentLength64 = responseText.Length; // Длина ответа
                        context.Response.OutputStream.Write(responseText, 0, responseText.Length); // Запись ответа
                        context.Response.OutputStream.Close();
                        return;
                    }

                    // Аналогично с Not Found
                    byte[] responseFile = File.ReadAllBytes(filePath); // Чтение байтов из файла
                    context.Response.ContentType = GetContentType(Path.GetExtension(filePath)); // Получение типа контента
                    context.Response.ContentEncoding = Encoding.UTF8;
                    context.Response.ContentLength64 = responseFile.Length;
                    context.Response.OutputStream.Write(responseFile, 0, responseFile.Length);
                    context.Response.OutputStream.Close();
                }
                catch
                {
                    Console.WriteLine("Сервер лёг, просыпайся"); // Сообщение о смерти сервера
                }
            }
            // Передача запроса дальше по цепи при наличии в ней обработчиков
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }

        private string GetContentType(string? extension) // Метод для получения типа контента
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension), "Extension cannot be null.");
            }

            return extension.ToLower() switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".ico" => "image/x-icon",
                ".svg" => "image/svg+xml",
                ".ttf" => "font/ttf",
                _ => "application/octet-stream",
            };
        }
    }
}
