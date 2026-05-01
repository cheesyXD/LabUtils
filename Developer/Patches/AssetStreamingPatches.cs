using HarmonyLib;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using LabUtils.Developer.Extensions;
using LabUtils.Utils.All.Developer;
using LabUtils.Utils.AssetWarehouseUtil;
using MelonLoader;
using UnityEngine;

namespace LabUtils.Developer.Patches
{
    [HarmonyPatch()]
    public static class AssetStreamingPatches
    {
        [HarmonyPatch(typeof(AssetWarehouse), nameof(AssetWarehouse.AddPallet))]
        [HarmonyPostfix]
        public static void AddPalletPostfix(Pallet pallet)
        {
            RetrievingTask.Crates.AddRange(pallet.Crates.ToList());
        }
        /*
[HarmonyPatch(typeof(SceneStreamer), nameof(SceneStreamer.Load), typeof(Barcode), typeof(Barcode))]
[HarmonyPostfix]
public static void Load1Postfix(Barcode levelBarcode, Barcode loadLevelBarcode)
{
    if (DebugUtility.SceneStreamerLogger.Value)
    {
        var message = $"Loading Level {levelBarcode.ID} with Load Level Barcode {loadLevelBarcode.ID} ";
        MelonLogger.Msg(message);
        DevUtils.Notify(message, 0.1f, "LabUtils-DEBUG");
    }
}

[HarmonyPatch(typeof(SceneStreamer), nameof(SceneStreamer.Load), typeof(LevelCrateReference), typeof(LevelCrateReference))]
[HarmonyPostfix]
public static void Load2Postfix(LevelCrateReference level, LevelCrateReference loadLevel)
{
    if (DebugUtility.SceneStreamerLogger.Value)
    {
        var message = $"Loading Level {level.Crate.Title} with Load Level {loadLevel.Crate.Title} ";
        MelonLogger.Msg(message);
        DevUtils.Notify(message, 0.1f, "LabUtils-DEBUG");
    }
}
[HarmonyPatch(typeof(AssetSpawner), nameof(AssetSpawner.Spawn))]
[HarmonyPostfix]
public static void SpawnPostfix(Spawnable spawnable, Vector3 position, Quaternion rotation, Il2CppSystem.Nullable<Vector3> scale, Transform parent, bool ignorePolicy, Il2CppSystem.Nullable<int> groupID, Il2CppSystem.Action<GameObject> spawnCallback, Il2CppSystem.Action<GameObject> despawnCallback)
{
    if (DebugUtility.AssetSpawnerLogger.Value)
    {
        var message = $"Spawning {spawnable.crateRef.Crate.Title}";
        MelonLogger.Msg(message);
        DevUtils.Notify(message, 0.1f, "LabUtils-DEBUG");
    }
}
*/
    }
}
