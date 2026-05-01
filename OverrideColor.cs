using UnityEngine;

public struct OverrideColor {
    public static Color lightBlue = GetColor("#5797ff");
    public static Color red = GetColor("#F54842");
    public static Color green = GetColor("#61ff61");
    public static Color GetColor(string hexcode) {
        if(ColorUtility.TryParseHtmlString(hexcode, out var color))
        {
            return color;
        }
        return Color.white;
    }
}