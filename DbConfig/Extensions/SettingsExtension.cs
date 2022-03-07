using DbConfig.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DbConfig.Extensions

{
    public static class SettingsExtension
    {
        public static Dictionary<string, string> GetDictionary(this AosCodes aosCodes)
        {
            return ParseDictionary(aosCodes).ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
        }

        public static Settings[] GetSettings(this AosCodes aosCodes)
        {
            return ParseDictionary(aosCodes).Select(kvp => new Settings(id: kvp.Key, value: kvp.Value)).ToArray();
        }

        private static IEnumerable<KeyValuePair<string, string>> ParseDictionary<T, Y>(Dictionary<T, Y> dictionary)
        {
            return dictionary.Select(element => element.Value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                               .ToDictionary(prop => $"{typeof(Y).Name}:{element.Key}:{prop.Name}"
                               , prop => prop.GetValue(element.Value).ToString()
                               , null)).SelectMany(i => i);
        }
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictA, IDictionary<TKey, TValue> dictB)
            where TValue : class
        {
            return dictA.Keys.Union(dictB.Keys).ToDictionary(k => k, k => dictA.ContainsKey(k) ? dictA[k] : dictB[k]);
        }
    }
}
