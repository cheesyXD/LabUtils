using HarmonyLib;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Data;
using Il2CppWebSocketSharp;
using LabUtils.Utils.InfiniteAmmoUtil;

namespace LabUtils.Developer.Patches
{
    public static class AmmoPatches
    {
        public static string[] groups = new string[3] {
            "light", "medium", "heavy"     
        };
        [HarmonyPatch(typeof(AmmoInventory), "Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(AmmoInventory _instance)
        {
            AmmoUtility.Inventory = _instance;
            foreach (var group in groups) {
                if (_instance.GetCartridgeCount(group) < 1) {
                    _instance.AddCartridge(_instance.lightAmmoGroup, 2000);
                    _instance.AddCartridge(_instance.mediumAmmoGroup, 2000);
                    _instance.AddCartridge(_instance.heavyAmmoGroup, 2000);
                }
            }
        }

        [HarmonyPatch(typeof(AmmoInventory), "RemoveCartridge")]
        [HarmonyPostfix]
        public static void RemoveCartridgePostfix(AmmoInventory __instance, CartridgeData cartridge, int count)
        {
            if (!AmmoUtility.UnlimitedAmmo.Value) return;

            var group = __instance.GetGroupByCartridge(cartridge);
            if (group.IsNullOrEmpty()) return;

            switch (group)
            {
                case "light":
                    __instance.AddCartridge(__instance.lightAmmoGroup, count);
                    break;
                case "medium":
                    __instance.AddCartridge(__instance.mediumAmmoGroup, count);
                    break;
                case "heavy":
                    __instance.AddCartridge(__instance.heavyAmmoGroup, count);
                    break;
            }
        }
    }
}
