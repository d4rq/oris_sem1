using System.Linq.Expressions;

namespace OrmLibrary;

public static class ExpressionParser<T> 
{
    internal static string ParseExpression(Expression expression)
    {
        if (expression is BinaryExpression binary)
        {
            // разбираем выражение на составляющие
            var left = ParseExpression(binary.Left);  // Левая часть выражения
            var right = ParseExpression(binary.Right); // Правая часть выражения
            var op = GetSqlOperator(binary.NodeType);  // Оператор (например, > или =)
            return $"({left} {op} {right})";
        }
        else if (expression is MemberExpression member)
        {
            return member.Member.Name; // Название свойства
        }
        else if (expression is ConstantExpression constant)
        {
            return FormatConstant(constant.Value); // Значение константы
        }
 
        // TODO: можно расширить для поддержки более сложных выражений (например, методов Contains, StartsWith и т.д.).
        // если не поддерживается то выбрасываем исключение
        throw new NotSupportedException($"Unsupported expression type: {expression.GetType().Name}");
    }
 
    internal static string GetSqlOperator(ExpressionType nodeType)
    {
        return nodeType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.LessThan => "<",
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",
            _ => throw new NotSupportedException($"Unsupported operator: {nodeType}")
        };
    }
 
    internal static string FormatConstant(object value)
    {
        return value is string ? $"'{value}'" : value.ToString();
    }

    internal static string BuildSqlQuery(Expression<Func<T, bool>> predicate, bool singleResult)
    {
        var query = $"SELECT * FROM {typeof(T).Name}s Where";
        query += ParseExpression(predicate.Body);
        return query;
    }
}