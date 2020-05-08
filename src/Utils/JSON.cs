//  Copyright © 2020 Randall Maas. All rights reserved.
using System.Collections.Generic;
using System.Text.Json;
namespace Anki.Vector.WebVizSDK
{
partial class Util
{
    /// <summary>
    /// Converts the JSON element to a C# object
    /// </summary>
    /// <param name="item">The JSON element</param>
    /// <returns>The new thing</returns>
    static object JsonToNormal(JsonElement item)
    {
        switch (item.ValueKind)
        {
            case JsonValueKind.Array : return toArray(item);
            case JsonValueKind.Object: return toDict(item);
            case JsonValueKind.String: return item.GetString();
            case JsonValueKind.True  : return true;
            case JsonValueKind.False : return false;
            case JsonValueKind.Number: return item.GetDouble();
        }
        return null;
    }

    /// <summary>
    /// Convert the JSON to a dictionary to an array of strings
    /// </summary>
    /// <param name="a">The dictionary of JSON elements</param>
    /// <returns>The dictionary</returns>
    internal static Dictionary<string, object> toDict(Dictionary<string, object> a)
    {
        var ret = new Dictionary<string, object>();
        foreach (var item in a)
        {
            ret[item.Key] = JsonToNormal((JsonElement) item.Value);
        }
        return ret;
    }

    /// <summary>
    /// Convert the JSON element to an a dictionary
    /// </summary>
    /// <param name="a">The JSON element</param>
    /// <returns>The dictionary</returns>
    static Dictionary<string, object> toDict(JsonElement a)
    {
        var ret = new Dictionary<string, object>();
        foreach (var item in a.EnumerateObject())
        {
            ret[item.Name] = JsonToNormal(item.Value);
        }
        return ret;
    }


    /// <summary>
    /// Convert the JSON element to an array
    /// </summary>
    /// <param name="v">The JSON object to make into a an array</param>
    /// <returns>The array</returns>
    static object[] toArray(JsonElement v)
    {
        var ret = new List<object>();
        foreach (var item in v.EnumerateArray())
            ret.Add(JsonToNormal(item));
        return ret.ToArray();
    }
}
}