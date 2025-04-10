using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ShowMatic.Server.Application.Extensions;
public static class FieldExtensions
{
    private const string _pattern = @"\[(.*?)\]";

    public static string SeInToFormula<T>(this T type, string formula, bool isFieldResult = false)
        where T : class
    {
        // check if type is list
        if (type is ICollection || type is IList || type is IEnumerable)
        {
            var list = type as IEnumerable;
            foreach (object? item in list)
            {
                formula = isFieldResult ? item.SeInToFormula("FieldId", "FieldValue", formula) : SetObjectValuesInFormula(item, formula);
            }

            return formula;
        }
        else
        {
            formula = isFieldResult ? type.SeInToFormula("FieldId", "FieldValue", formula) : SetObjectValuesInFormula(type, formula);
        }

        return formula;
    }

    private static string SetObjectValuesInFormula<T>(T type, string formula)
        where T : class
    {
        foreach (string extractedItem in ExtractItemsByPattern(formula, _pattern))
        {
            object? value = type.GetValueByPath(extractedItem);
            if (value != null && !string.IsNullOrEmpty(value?.ToString()))
            {
                formula = formula.Replace("[" + extractedItem + "]", value?.ToString());
            }
        }

        return formula;
    }

    public static string SeInToFormula<T>(this T type, string propertyToBeSearched, string propertyToBeReplaced, string formula)
        where T : class
    {
        foreach (string token in ExtractItemsByPattern(formula, _pattern))
        {
            object? sourceValue = type.GetValueForObject(propertyToBeSearched);
            if (sourceValue?.ToString()?.ToLower() == token.ToLower())
            {
                object destinationValue = type.GetValueForObject(propertyToBeReplaced) ?? string.Empty;
                formula = formula.Replace("[" + token + "]", destinationValue.ToString());
            }
        }

        return formula;
    }

    public static List<string> ExtractItemsByPattern(string input, string pattern)
    {
        var items = new List<string>();
        foreach (var match in Regex.Matches(input, pattern).Cast<Match>())
        {
            items.Add(match.Groups[1].Value);
        }

        return items;
    }

    public static object? GetValueByPath(this object? obj, string path)
    {
        if (obj == null || string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        foreach (string propName in path.Split('.'))
        {
            var typeProperties = obj?.GetType().GetProperties()
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            var propInfo = SearchPropertyInfo(obj, propName);
            obj = propInfo?.GetValue(obj);
        }

        return obj;
    }

    public static object? GetValueForObject(this object? obj, string path)
    {
        if (obj == null || string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        foreach (string propName in path.Split('.'))
        {
            var typeProperties = obj?.GetType().GetProperties()
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            var propInfo = SearchPropertyInfo(obj, propName);
            try
            {
                obj = propInfo?.GetValue(obj);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        return obj;
    }

    public static PropertyInfo? SearchPropertyInfo(object? obj, string propName)
    {
        var typeProperties = obj?.GetType().GetProperties()?
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
        if (typeProperties?.TryGetValue(propName, out var propInfo) == true)
        {
            return propInfo;
        }

        return null;
    }
}