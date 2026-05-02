using UnityEngine;

public struct ColorPlus {
    private static Color[] colors = { GetColor("#F54842"), GetColor("#ff8c00"), GetColor("#61ff61"), GetColor("#5797ff"), GetColor("#bf00ff"), };

    private static byte _nextId = 0;
    private static byte nextId
    {
        get
        {
            if (_nextId >= colors.Length) _nextId = 0;
            return _nextId++;
        }
    }
    public static Color GetColor(string hexcode) {
        if(ColorUtility.TryParseHtmlString(hexcode, out var color))
        {
            return color;
        }
        return Color.white;
    }

    public static Color GetNextColor()
    {
        return colors[nextId];
    }
}