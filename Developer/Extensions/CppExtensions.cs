namespace LabUtils.Developer.Extensions {
    public static class CppExtensions {
        public static List<T> ToList<T>(this Il2CppSystem.Collections.Generic.List<T> dirtyList) {
            // idk what this does, vs just gave me this
            // Hashtag Vibe Code bullshit
            List<T> list = [.. dirtyList];
            dirtyList.Clear();
            return list;
        }
    }
}
