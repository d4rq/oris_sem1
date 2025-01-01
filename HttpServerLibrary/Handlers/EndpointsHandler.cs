using System.Reflection;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponse;

namespace HttpServerLibrary.Handlers;

internal sealed class EndpointsHandler : Handler
{
    private readonly Dictionary<string, List<(HttpMethod method, MethodInfo methodInfo, Type endpointType)>> _routes = new(); // Словарь с названиями методов и информацией о них

    public EndpointsHandler()
    {
        RegisterEndpointsFromAssemblies([Assembly.GetEntryAssembly()!]); // Получение эндпоинтов из сборки
    }

    public override void HandleRequest(HttpRequestContext context)
    {
        var request = context.Request;
        var url = request.Url!.LocalPath.Trim('/'); // Получение названия эндпоинта
        var requestMethod = context.Request.HttpMethod; // Получение метода

        if (_routes.ContainsKey(url)) // Проверка на существование эндпоинта
        {
            var route = _routes[url].FirstOrDefault(r => r.method.ToString().Equals(requestMethod, StringComparison.OrdinalIgnoreCase)); // Получение информации о методе

            if (route.methodInfo != null)
            {
                var endpointInstance = Activator.CreateInstance(route.endpointType) as EndpointBase; // Создание экземпляра эндпоинта

                if (endpointInstance != null)
                {
                    endpointInstance.SetContext(context); // Задание контекста для эндпоинта

                    var parameters = GetMethodParameters(route.methodInfo, context); // Получение информации о параметрах метода-обработчика эндпоинта
                    var result = route.methodInfo.Invoke(endpointInstance, parameters) as IHttpResponseResult; // Результат работы метода
                    result?.Execute(context); // Отправка результата клиенту
                }
            }
        }
        else if (Successor != null) // Передача управления следующему обработчику
        {
            // TODO: добавить Handler 404 ошибки
            Successor.HandleRequest(context);
        }
    }

    private void RegisterEndpointsFromAssemblies(Assembly[] assemblies) // Магия рефлексии
    {
        foreach (Assembly assembly in assemblies) // Мотаемся по сборкам
        {
            var endpointTypes = assembly.GetTypes()
                .Where(t => typeof(EndpointBase).IsAssignableFrom(t) && !t.IsAbstract); // Получение типов эндпоинтов
            foreach (var endpointType in endpointTypes)
            {
                var methods = endpointType.GetMethods(); // Получение методов контроллера эндпоинта
                foreach (var methodInfo in methods)
                {
                    // Сопоставление названий эндпоинтов с их обработчиками
                    var getAttribute = methodInfo.GetCustomAttribute<GetAttribute>();
                    if (getAttribute != null)
                    {
                        RegisterRoute(getAttribute.Route.ToLower(), HttpMethod.Get, methodInfo, endpointType);
                    }

                    var postAttribute = methodInfo.GetCustomAttribute<PostAttribute>();
                    if (postAttribute != null)
                    {
                        RegisterRoute(postAttribute.Route.ToLower(), HttpMethod.Post, methodInfo, endpointType);
                    }
                }
            }
        }
    }

    private void RegisterRoute(string route, HttpMethod method, MethodInfo methodInfo, Type endpointType) // Сама логика сопоставлений
    {

        if (!_routes.ContainsKey(route)) // Проверка на существование того же эндпоинта
        {
            _routes[route] = new();
        }

        if (_routes.ContainsKey(route)
            && _routes.GetValueOrDefault(route)!.Select(x => x.method).Contains(method)) // В случае совпадения названий проверка на совпадение методов
            throw new ArgumentException($"Эндпоинт {route} с методом {method.Method} уже существует");

        _routes[route].Add((method, methodInfo, endpointType)); // Если ничего не полетело, добавление соответствия
    }

    private object[] GetMethodParameters(MethodInfo method, HttpRequestContext context) // Получение параметров из запроса для передачи их в метод
    {
        var parameters = method.GetParameters(); // Получение информации о параметрах
        var values = new object[parameters.Length]; // Массив объектов для передачи в метод Invoke()

        if (context.Request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase)) // Если GET - получаем параметры из строки запроса
        {
            // Извлекаем параметры из строки запроса
            var queryParameters = System.Web.HttpUtility.ParseQueryString(context.Request.Url!.Query); // Парсинг сроки запроса

            for (int i = 0; i < parameters.Length; i++) // Перебор параметров с присвоением значений
            {
                var paramName = parameters[i].Name;
                var paramType = parameters[i].ParameterType;
                var value = queryParameters[paramName];
                values[i] = ConvertValue(value!, paramType); // Конвертация параметра в нужный принимаемый тип
            }
        }
        else if (context.Request.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase)) // Если POST - получаем параметры из тела запроса
        {
            // Извлекаем параметры из тела запроса
            using var reader = new StreamReader(context.Request.InputStream); // Чтение тела запроса
            var body = reader.ReadToEnd();

            if (context.Request.ContentType == "application/x-www-form-urlencoded") // Проверка на получение данных из формы
            {
                var formParameters = System.Web.HttpUtility.ParseQueryString(body); // Парсинг тела запроса

                for (int i = 0; i < parameters.Length; i++)
                {
                    var paramName = parameters[i].Name;
                    var paramType = parameters[i].ParameterType;
                    var value = formParameters[paramName];
                    values[i] = ConvertValue(value!, paramType); // Конвертация параметра в нужный принимаемый тип
                }
            }
            else if (context.Request.ContentType == "application/json")
            {
                var jsonObject = System.Text.Json.JsonSerializer.Deserialize(body, method.GetParameters()[0].ParameterType); // Парсинг JSON в объект
                return [jsonObject!];
            }
        }

        return values;
    }

    private object ConvertValue(string value, Type targetType) // Самодельный метод конвертации в нужный тип
    {
        if (value == null)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType)! : null!;
        }

        return Convert.ChangeType(value, targetType);
    }
}