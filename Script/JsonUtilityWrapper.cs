using System.Collections.Generic;
using UnityEngine;

public static class JsonUtilityWrapper
{
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }

    public static List<T> FromJsonList<T>(string json)
    {
        return JsonUtility.FromJson<Wrapper<T>>("{\"items\":" + json + "}").items;
    }

    public static string ToJsonList<T>(List<T> list)
    {
        return JsonUtility.ToJson(new Wrapper<T> { items = list });
    }
}