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

        [HarmonyPatch(typeof(Player_Health), nameof(Player_Health.SetFullHealth))]
        [HarmonyPostfix]
        public static void SetFullHealthPostfix()
        {
            RagdollUtility.UnRagdoll();
        }

        [HarmonyPatch(typeof(Player_Health), nameof(Player_Health.Death))]
        [HarmonyPrefix]
        public static bool DeathPrefix()
        {
            return !HealthUtility.GodMode.Value;
        }

        [HarmonyPatch(typeof(Player_Health), nameof(Player_Health.Death))]
        [HarmonyPostfix]
        public static void DeathPostfix()
        {
            if(RagdollUtility.RagdollOnDeath.Value) RagdollUtility.Ragdoll();
        }
    }
}
