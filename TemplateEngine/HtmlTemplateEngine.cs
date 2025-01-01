using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TemplateEngine;

public class HtmlTemplateEngine : IHtmlTemplateEngine
{
    public string Render(string template, string data)
    {
        return template.Replace("{name}", data);
    }
    
    public string Render(string template, object obj)
    {
       var properties = obj.GetType().GetProperties();
       var result = template;
       result = HandleVariables(result, obj);
       result = HandleConditionals(result, properties, obj);
       result = HandleLoops(result,properties, obj);
       return result;
    }
    
    private string HandleVariables(string template, object obj)
    {
        var type = obj.GetType();
        var result = template;
        var properties = type.GetProperties(BindingFlags.Public|BindingFlags.Instance);
        foreach (var prop in properties)
        {
            var repStr = "{" + prop.Name.ToLower() + "}";
            var value = prop.GetValue(obj).ToString();
            result = result.Replace(repStr, value);
        }

        return result;
    }
    // <{{Value}}{<<operator>>}{{Value}}>
    private string HandleConditionals(string template, PropertyInfo[] properties, object model)
    {
        var pattern = @"{%if<(.+?)>%}(.*?)({%else%}(.*?))?{%/if%}";
        var regex = new Regex(pattern, RegexOptions.Singleline);
        return new string(regex.Replace(template, match =>
        {
            var conditional = match.Groups[1].Value.Trim();
            var content = match.Groups[2].Value.Trim(); //трим опционально можно убрать
            var elseContent = match.Groups[3].Success ? match.Groups[4].Value.Trim() : null;
            //var property = properties.FirstOrDefault(p => p.Name == conditional);
            //if (property is not null && property.GetValue(model) is bool value && value)
            if(ProcessCondition(conditional, properties, model))
            {
                return Render(content, model);
            }
            if(elseContent != null)
            {
                return Render(elseContent, model);
            }
            return String.Empty;
        }));
    }

    private bool ProcessCondition(string condition, PropertyInfo[] properties, object model)
    {
        foreach (var property in properties)
        {
            var value = property.GetValue(model).ToString();
            var pattern = $@"{{{{{property.Name}}}}}";
            condition = Regex.Replace(condition, pattern, value, RegexOptions.IgnoreCase);
        }

        return (bool)new System.Data.DataTable().Compute(condition, string.Empty);
    }

    private string HandleLoops(string template, PropertyInfo[] properties, object model)
    {
        var pattern = @"{%for%(.+?)%in%(.+?)}(.*?){%/for%}";
        var regexp = new Regex(pattern);
        return new string(regexp.Replace(template, match =>
        {
            var item = match.Groups[1].Value.Trim();
            var collection = match.Groups[2].Value.Trim();
            var content = match.Groups[3].Value;
            var values = properties.FirstOrDefault(p => p.Name == collection).GetValue(model);
            if (values is IEnumerable)
            {
                var loopResult = new StringBuilder();
              foreach (var value in (IEnumerable)(values))
              {
                  var subTemplate = content.Replace("{{" + item + ".", "{{");
                  loopResult.Append(Render(subTemplate, value));
              }

              return loopResult.ToString();
            }
            return string.Empty;
        }));
    }
}