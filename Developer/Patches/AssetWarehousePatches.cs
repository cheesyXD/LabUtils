using HarmonyLib;
using Il2CppSLZ.Marrow.Warehouse;

namespace LabUtils.Developer.Patches
{
    [HarmonyPatch(typeof(AssetWarehouse))]
    public static class AssetWarehousePatches
    {
        [HarmonyPatch(nameof(AssetWarehouse.AddCrate))]
        [HarmonyPostfix]
        public static void AddCratePostfix(Crate crate)
        {
            LabData.crates.Add(crate);
        }
    }
}
