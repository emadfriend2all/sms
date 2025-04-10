using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShowMatic.Server.Application.Extensions;
public static class Shared
{
    public static DataTable ConvertListToDataTable<T>(this IList<T> data)
    {
        DataTable table = new DataTable("table", "table");
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        foreach (T item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            table.Rows.Add(row);
        }

        return table;
    }

    public static DataTable ConvertToDataTable<T>(this T data)
    {
        DataTable table = new DataTable(nameof(data), nameof(data));
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

        DataRow row = table.NewRow();
        foreach (PropertyDescriptor prop in properties)
            row[prop.Name] = prop.GetValue(data) ?? DBNull.Value;
        table.Rows.Add(row);

        return table;
    }

    public static string GetPath<T>(Expression<Func<T>> action)
    {
        try
        {
            string path = action.Body.ToString();
            var list = path.Split('.').ToList();
            list.RemoveRange(0, 2);
            string result = string.Join('.', list);
            return result;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public static string GetObjectPath(this object? obj, string currentPath = "")
    {
        if (obj == null)
        {
            return currentPath;
        }

        if (!string.IsNullOrEmpty(currentPath))
        {
            currentPath += ".";
        }

        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();

        if (properties.Length == 0)
        {
            // This is a leaf node with no properties, return the current path
            return currentPath + type.Name;
        }

        // Recursively traverse properties
        foreach (PropertyInfo property in properties)
        {
            object? propertyValue = property.GetValue(obj);
            string newPath = currentPath + property.Name;
            currentPath = GetObjectPath(propertyValue, newPath);
        }

        return currentPath;
    }

    public static IEnumerable<PropertyInfo> GetCollectionsInObject(this Type type)
    {
        // Get properties with PropertyType declared as interface
        var interfaceProps =
            from prop in type.GetProperties()
            from interfaceType in prop.PropertyType.GetInterfaces()
            where interfaceType.IsGenericType
            let baseInterface = interfaceType.GetGenericTypeDefinition()
            where (baseInterface == typeof(ICollection<>)) || (baseInterface == typeof(ICollection))
            select prop;

        // Get properties with PropertyType declared(probably) as solid types.
        var nonInterfaceProps =
            from prop in type.GetProperties()
            where typeof(ICollection).IsAssignableFrom(prop.PropertyType) || typeof(ICollection<>).IsAssignableFrom(prop.PropertyType)
            select prop;

        // Combine both queries into one resulting
        return interfaceProps.Union(nonInterfaceProps);
    }

    public static bool AreEqual<T>(this T obj1, T obj2)
    {
        if (obj1 == null && obj2 == null)
        {
            return true;
        }
        else if (obj1 == null || obj2 == null)
        {
            return false;
        }
        else if (obj1.GetType() != obj2.GetType())
        {
            return false;
        }
        else
        {
            PropertyInfo[] properties = obj1.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                object? value1 = property.GetValue(obj1);
                object? value2 = property.GetValue(obj2);

                if (value1 == null && value2 == null)
                {
                    continue;
                }
                else if (value1 == null || value2 == null)
                {
                    return false;
                }
                else if (!value1.Equals(value2))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public static List<string> ExtractHashtags(this string input)
    {
        List<string> hashtags = new List<string>();
        if (string.IsNullOrEmpty(input))
        {
            return hashtags;
        }

        string pattern = @"#(\w+)";

        MatchCollection matches = Regex.Matches(input, pattern);

        foreach (Match match in matches)
        {
            string hashtag = match.Groups[1].Value;
            hashtags.Add(hashtag);
        }

        return hashtags;
    }
}
