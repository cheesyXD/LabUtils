using Harmony;

namespace LabUtils.Developer.Extensions {
    public static class CppExtensions {
        public static List<T> ToList<T>(this Il2CppSystem.Collections.Generic.List<T> dirtyList) {
            List<T> list = [.. dirtyList];
            return list;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue> dirtyDictionary)
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            foreach (var item in dirtyDictionary)
            {
                dictionary.Add(item.Key, item.Value);
            }
            return dictionary;
        }
    }
}
