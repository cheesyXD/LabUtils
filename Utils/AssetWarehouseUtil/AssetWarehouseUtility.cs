using BoneLib;
using BoneLib.BoneMenu;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Props;
using LabUtils.Developer;
using MelonLoader;
using System.Text.Json.Serialization;
using UnityEngine;
using Page = BoneLib.BoneMenu.Page;

namespace LabUtils.Utils.AssetWarehouseUtil
{
    public class AssetWarehouseUtility : Utility
    {
        public static Page Page;
        public static Page ResultsPage;
        public static Page SelectedPage;
        public static MelonPreferences_Entry<bool> ShowRedacted { get; set; } = Core.Preferences.CreateEntry<bool>("ShowRedacted", true);
        public static MelonPreferences_Entry<bool> ShowUnlockable { get; set; } = Core.Preferences.CreateEntry<bool>("ShowUnlockable", true);

        public static MelonPreferences_Entry<bool> IncludeTags { get; set; } = Core.Preferences.CreateEntry<bool>("IncTags", true);
        protected override void OnLoad()
        {
            Page = UICore.UtilitiesPage.CreatePage("Asset Warehouse", OverrideColor.red);
            var settings = Page.CreatePage("Settings", Color.white);
            settings.CreateBool("Show Redacted", Color.white, ShowRedacted.Value, (a) => ShowRedacted.Value = a);
            settings.CreateBool("Show Unlockable", Color.white, ShowUnlockable.Value, (a) => ShowUnlockable.Value = a);
            settings.CreateBool("Include Tags", Color.white, IncludeTags.Value, (a) => IncludeTags.Value = a);
            Page.CreateString("Search Query", OverrideColor.green, "Ford", OnSearched);
            ResultsPage = Page.CreatePage("Results", OverrideColor.lightBlue, maxElements: 5);
            SelectedPage = Page.CreatePage("Selected Crate", OverrideColor.lightBlue);
        }

        private static void OnSearched(string value)
        {
            RetrievingTask.RetrieveResults(value, ReceiveResults);
        }

        internal static void ReceiveResults(CrateRep[] reps)
        {
            ResultsPage.RemoveAll();
            foreach (var rep in reps)
            {
                ResultsPage.CreateFunction(rep.title, OverrideColor.lightBlue, () => OnClickCrate(rep)).SetTooltip(rep.ToString());
            }
            DevUtils.Notify("Retrieved Results - Asset Warehouse");
            RetrievingTask.isRunning = false;
        }
        private static void OnClickCrate(CrateRep rep)
        {
            SelectedPage.RemoveAll();
            SelectedPage.CreateFunction($"{rep.title}", Color.white, null).SetTooltip(rep.ToString());
            switch (rep.type)
            {
                case CrateType.Level:
                    SelectedPage.CreateFunction("Load Level", OverrideColor.red, ()=>SceneStreamer.Load(new(rep.barcode)));
                    break;
                case CrateType.Avatar:
                    SelectedPage.CreateFunction("Change Avatar", OverrideColor.red, () => Player.RigManager.SwapAvatarCrate(new(rep.barcode)));
                    break;
                case CrateType.Spawnable:
                    SelectedPage.CreateFunction("Swap Spawn Gun Crate", OverrideColor.red, () => SwapSpawnGunCrate(rep.barcode));
                    break;
            }
            SelectedPage.CreateFunction("Load Crate Assets into Memory", OverrideColor.red, () =>
            {
                if (!AssetWarehouse.Instance.TryGetCrate(new Barcode(rep.barcode), out Crate crate)) return;
                crate.PreloadAssets();
            });
            SelectedPage.CreateFunction("Unlock Crate", OverrideColor.red, () =>
            {
                GachaCapsule.u.IncrementUnlockForBarcode(new Barcode(rep.barcode));
            });
            SelectedPage.CreateFunction("Lock Crate", OverrideColor.red, () =>
            {
                GachaCapsule.u.ClearUnlockForBarcode(new Barcode(rep.barcode));
            });
            DevUtils.Notify("Selected Crate!");
        }

        private static void SwapSpawnGunCrate(string barcode)
        {
            if (!AssetWarehouse.Instance.TryGetCrate(new Barcode(barcode), out SpawnableCrate crate)) return;
            var spawnGuns = UnityEngine.Object.FindObjectsOfType<SpawnGun>();
            foreach (var spawnGun in spawnGuns) {
                spawnGun._lastSelectedCrate = crate;
                spawnGun._selectedCrate = crate;
            }
        }
    }
}
