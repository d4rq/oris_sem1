using System.Net;
using HttpServerLibrary.Configuration;
using HttpServerLibrary.Handlers;

namespace HttpServerLibrary
{
    public class HttpServer
    {
        private readonly HttpListener _listener;

        private AppConfig config = AppConfig.GetInstance(); // Вытаскиваем конфиг
        Handler h1 = new StaticFilesHandler(); // Обработчик файлов
        Handler h2 = new EndpointsHandler(); // Обработчик эндпоинтов

        public HttpServer()
        {
            _listener = new HttpListener(); // Инициализация listener-a
            _listener.Prefixes.Add($"http://{config.Domain}:{config.Port}/"); // Добавление префиксов прослушивания
        }

        public void Start()
        {
            _listener.Start(); // Запуск listener-a
            Console.WriteLine("Сервер работает на " + _listener.Prefixes.First());

            HandleRequests(); // Метод для постоянной обработки запросов
        }

        public void HandleRequests()
        {
            while (_listener.IsListening)
            {
                var context = _listener.GetContext(); // Получение контекста

                ProcessRequest(context); // Метод обработки каждого отделного запроса
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {

            h1.Successor = h2; // Цепочка обязанностей - если метод первого обработчика не срабатывает из-за некоторых условий, управление передается его преемнику
            h1.HandleRequest(new HttpRequestContext(context.Request, context.Response));
        }

        public void Stop()
        {
            _listener.Stop(); // Отсновка listener-a
            Console.WriteLine("Сервер остановлен");
        }
    }
}
