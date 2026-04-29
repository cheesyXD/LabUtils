using HarmonyLib;
using Il2CppSLZ.Marrow.Warehouse;
using LabUtils.Developer.Extensions;
using LabUtils.Utils.AssetWarehouseUtil;

namespace LabUtils.Developer.Patches
{
    [HarmonyPatch(typeof(AssetWarehouse))]
    public static class AssetWarehousePatches
    {
        [HarmonyPatch(nameof(AssetWarehouse.AddPallet))]
        [HarmonyPostfix]
        public static void AddPalletPostfix(Pallet pallet)
        {
            RetrievingTask.Crates.AddRange(pallet.Crates.ToList());
        }
    }
}
