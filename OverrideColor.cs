using UnityEngine;

public struct OverrideColor {
    public static Color lightBlue = GetColor("#5797ff");
    public static Color red = GetColor("#F54842");
    public static Color green = GetColor("#61ff61");
    public static Color GetColor(string hexcode) {
        ColorUtility.DoTryParseHtmlColor(hexcode, out var color);
        return color;
    }
}