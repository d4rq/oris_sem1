namespace HttpServerLibrary.Handlers
{
    public abstract class Handler
    {
        public Handler? Successor { get; set; } // Обработчик-преемник
        public abstract void HandleRequest(HttpRequestContext context); // Абстрактный метод обработки запроса
    }
}
