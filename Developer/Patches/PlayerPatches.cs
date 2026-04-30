using HarmonyLib;
using Il2CppSLZ.Marrow;
using LabUtils.Utils.All;

namespace LabUtils.Developer.Patches
{
    [HarmonyPatch]
    public static class PlayerPatches
    {
        [HarmonyPatch(typeof(Player_Health), nameof(Player_Health.TAKEDAMAGE))]
        [HarmonyPrefix]
        public static bool TakeDamagePrefix(ref float damage)
        {
            return !HealthUtility.GodMode.Value;
        }
    }
}
