using BoneLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSystem.Xml;
using LabUtils.Developer;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using Page = BoneLib.BoneMenu.Page;

namespace LabUtils.Utils.AssetWarehouseUtil
{
    public class AssetWarehouseUtility : Utility
    {
        public static Page Page;
        public static Page ResultsPage;
        public static Page SelectedPage;
        public static Page FavoritesPage;
        public static MelonPreferences_Entry<bool> ShowRedacted { get; set; } = Core.Preferences.CreateEntry<bool>("ShowRedacted", true);
        public static MelonPreferences_Entry<bool> ShowUnlockable { get; set; } = Core.Preferences.CreateEntry<bool>("ShowUnlockable", true);

        public static MelonPreferences_Entry<bool> IncludeTags { get; set; } = Core.Preferences.CreateEntry<bool>("IncTags", true);
        public static MelonPreferences_Entry<string[]> FavoriteCrates { get; set; } = Core.Preferences.CreateEntry("Favorites", new string[0]);
        protected override void OnLoad()
        {
            Page = UICore.UtilitiesPage.CreatePage("Asset Warehouse", ColorPlus.GetNextColor(), maxElements: 10);
            var settings = Page.CreatePage("Settings", ColorPlus.GetNextColor(), maxElements: 10);
            settings.CreateBool("Show Redacted", ColorPlus.GetNextColor(), ShowRedacted.Value, (a) => ShowRedacted.Value = a);
            settings.CreateBool("Show Unlockable", ColorPlus.GetNextColor(), ShowUnlockable.Value, (a) => ShowUnlockable.Value = a);
            settings.CreateBool("Include Tags", ColorPlus.GetNextColor(), IncludeTags.Value, (a) => IncludeTags.Value = a);
            Page.CreateString("Search Query", ColorPlus.GetNextColor(), "Ford", OnSearched);
            ResultsPage = Page.CreatePage("Results",  ColorPlus.GetNextColor(), maxElements: 10);
            SelectedPage = Page.CreatePage("Selected Crate",  ColorPlus.GetNextColor(), maxElements: 10);
            FavoritesPage = Page.CreatePage("Favorite Crates", ColorPlus.GetNextColor(), maxElements: 10);
            Hooking.OnWarehouseReady += Hooking_OnWarehouseReady;
        }

        private void Hooking_OnWarehouseReady()
        {
            RefreshFavoritesPage();
        }

        private static void RefreshFavoritesPage()
        {
            Core.Save();
            FavoritesPage.RemoveAll();
            foreach (var barcode in FavoriteCrates.Value)
            {
                if (!AssetWarehouse.Instance.TryGetCrate(new(barcode), out var crate)) continue;
                var rep = CrateRep.CreateCrateRep(crate);
                FavoritesPage.CreateFunction(rep.title, ColorPlus.GetNextColor(), () => OnClickCrate(rep)).SetTooltip(rep.ToString());
            }
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
                ResultsPage.CreateFunction(rep.title, ColorPlus.GetNextColor(), () => OnClickCrate(rep)).SetTooltip(rep.ToString());
            }
            DevUtils.Notify("Retrieved Results - Asset Warehouse");
            RetrievingTask.isRunning = false;
        }
        private static void OnClickCrate(CrateRep rep)
        {
            SelectedPage.RemoveAll();
            SelectedPage.CreateFunction($"{rep.title}", ColorPlus.GetNextColor(), null).SetTooltip(rep.ToString());
            switch (rep.type)
            {
                case CrateType.Level:
                    SelectedPage.CreateFunction("Load Level", ColorPlus.GetNextColor(), () => SceneStreamer.Load(new(rep.barcode)));
                    DevUtils.Notify("Level Loaded");
                    break;
                case CrateType.Avatar:
                    SelectedPage.CreateFunction("Swap Avatar", ColorPlus.GetNextColor(), () => Player.RigManager.SwapAvatarCrate(new(rep.barcode)));
                    DevUtils.Notify("Avatar Swapped");
                    break;
                case CrateType.Spawnable:
                    SelectedPage.CreateFunction("Swap Spawn Gun Crate", ColorPlus.GetNextColor(), () => SwapSpawnGunCrate(rep.barcode));
                    DevUtils.Notify("Spawn Gun Crate Swapped");
                    break;
            }
            SelectedPage.CreateFunction("Load Crate Assets into Memory", ColorPlus.GetNextColor(), () =>
            {
                if (!AssetWarehouse.Instance.TryGetCrate(new Barcode(rep.barcode), out Crate crate)) return;
                crate.PreloadAssets();
                DevUtils.Notify("Preloading Crate");
            });
            SelectedPage.CreateFunction("Unlock Crate", ColorPlus.GetNextColor(), () =>
            {
                DataManager.ActiveSave.Unlocks.IncrementUnlockForBarcode(new Barcode(rep.barcode));
                DataManager.TrySaveActiveSave(Il2CppSLZ.Marrow.SaveData.SaveFlags.DefaultAndPlayerSettingsAndUnlocks);
                DevUtils.Notify("Crate Unlocked");
            });
            SelectedPage.CreateFunction("Lock Crate",  ColorPlus.GetNextColor(), () =>
            {
                DataManager.ActiveSave.Unlocks.ClearUnlockForBarcode(new Barcode(rep.barcode));
                DataManager.TrySaveActiveSave(Il2CppSLZ.Marrow.SaveData.SaveFlags.DefaultAndPlayerSettingsAndUnlocks);
                DevUtils.Notify("Crate Locked");
            });
            SelectedPage.CreateFunction("Favorite Crate",  ColorPlus.GetNextColor(), () =>
            {
                var crates = FavoriteCrates.Value.ToList();
                if(!crates.Contains(rep.barcode)) crates.Add(rep.barcode);
                FavoriteCrates.Value = crates.ToArray();
                DevUtils.Notify("Crate Favorited");
                RefreshFavoritesPage();
            });
            SelectedPage.CreateFunction("UnFavorite Crate",  ColorPlus.GetNextColor(), () =>
            {
                var crates = FavoriteCrates.Value.ToList();
                if (crates.Contains(rep.barcode)) crates.Remove(rep.barcode);
                FavoriteCrates.Value = crates.ToArray();
                DevUtils.Notify("Crate removed from Favorites");
                RefreshFavoritesPage();
            });
            DevUtils.Notify("Selected Crate!");
        }

        private static void SwapSpawnGunCrate(string barcode)
        {
            if (!AssetWarehouse.Instance.TryGetCrate(new Barcode(barcode), out SpawnableCrate crate)) return;
            var spawnGuns = UnityEngine.Object.FindObjectsOfType<SpawnGun>();
            foreach (var spawnGun in spawnGuns)
            {
                spawnGun._lastSelectedCrate = crate;
                spawnGun._selectedCrate = crate;
            }
        }
    }
}
