using HarmonyLib;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Data;
using LabUtils.Utils.InfiniteAmmoUtil;

namespace LabUtils.Developer.Patches
{
    [HarmonyPatch()]
    public static class AmmoPatches
    {
        public static string[] groups =  {
            "light", "medium", "heavy"
        };
        [HarmonyPatch(typeof(AmmoInventory), nameof(AmmoInventory.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix()
        {
            foreach (var group in groups)
            {
                if (AmmoInventory.Instance.GetCartridgeCount(group) < 1)
                {
                    AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.lightAmmoGroup, 2000);
                    AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.mediumAmmoGroup, 2000);
                    AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.heavyAmmoGroup, 2000);
                }
            }
        }

        [HarmonyPatch(typeof(AmmoInventory), nameof(AmmoInventory.RemoveCartridge))]
        [HarmonyPrefix]
        public static bool RemoveCartridgePrefix(CartridgeData cartridge, int count)
        {
            return !AmmoUtility.UnlimitedAmmo.Value;
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.OnFire))]
        [HarmonyPrefix]
        public static void OnFirePrefix(Gun __instance)
        {
            if (AmmoUtility.UnlimitedMagazines.Value)
            {
                __instance?.gameObject?.GetComponentInChildren<Magazine>()?.magazineState?.Refill();
            }
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.AmmoCount))]
        [HarmonyPrefix]
        public static bool ShotgunCreditCardPrefix(Gun __instance, ref int __result) // DO NOT CHANGE __instance OR __result TO ANYTHING ELSE
        {
            if (AmmoUtility.UnlimitedShells.Value)
            {
                __result = 1;
                return false;
            }
            else
            {
                return true;
            }
        }
        [HarmonyPatch(typeof(Magazine), nameof(Magazine.OnGrab))]
        [HarmonyPrefix]
        public static void OnGrabPrefix(Magazine __instance, Hand hand)
        {
            if (AmmoUtility.GripMagazineRefill.Value)
            {
                __instance.magazineState.Refill();
            }
        }
    }
}
