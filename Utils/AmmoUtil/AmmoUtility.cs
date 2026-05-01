using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow;
using LabUtils.Developer;
using LabUtils.Developer.Patches;
using MelonLoader;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LabUtils.Utils.InfiniteAmmoUtil
{
    public class AmmoUtility : Utility
    {
        public static Page Page;
        public static MelonPreferences_Entry<bool> UnlimitedAmmo { get; set; } = Core.Preferences.CreateEntry<bool>("UnlimitedAmmo", false);
        public static MelonPreferences_Entry<bool> UnlimitedMagazines { get; set; } = Core.Preferences.CreateEntry<bool>("UnlimitedMagazines", false);
        public static MelonPreferences_Entry<bool> UnlimitedShells { get; set; } = Core.Preferences.CreateEntry<bool>("UnlimitedShells", false);
        public static MelonPreferences_Entry<bool> GripMagazineRefill { get; set; } = Core.Preferences.CreateEntry<bool>("GripMagazineRefill", false);
        protected override void OnLoad()
        {
            string name = Random.Range(0, 100) > 90 ? "AmmoLab" : "Ammo Utility";
            Page = UICore.UtilitiesPage.CreatePage(name, OverrideColor.red, maxElements: 10);
            Page.CreateBool("Unlimited Ammo", Color.white, UnlimitedAmmo.Value, VeryHappyAmmoMode);
            Page.CreateBool("Unlimited Magazines", Color.white, UnlimitedMagazines.Value, (a) => UnlimitedMagazines.Value = a);
            Page.CreateBool("Unlimited Shells/Slugs/Cartridges", Color.white, UnlimitedShells.Value, (a) => UnlimitedShells.Value = a);
            Page.CreateBool("Grip Magazine Refill", Color.white, GripMagazineRefill.Value, (a) => GripMagazineRefill.Value = a);
        }

        private void VeryHappyAmmoMode(bool obj)
        {
            UnlimitedAmmo.Value = obj;
            foreach (var group in AmmoPatches.groups)
            {
                if (AmmoInventory.Instance.GetCartridgeCount(group) < 1)
                {
                    AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.lightAmmoGroup, 2000);
                    AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.mediumAmmoGroup, 2000);
                    AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.heavyAmmoGroup, 2000);
                }
            }
            DevUtils.Notify(obj ? "Very Happy Ammo Added" : "Very Happy Ammo Removed", 1f);
        }
    }
}
