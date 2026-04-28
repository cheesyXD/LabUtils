using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow;
using LabUtils.Developer;
using MelonLoader;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LabUtils.Utils.InfiniteAmmoUtil
{
    public class AmmoUtility : Utility
    {
        public static Page Page;
        public static AmmoInventory Inventory;
        public static MelonPreferences_Entry<bool> UnlimitedAmmo { get; set; } = Core.Preferences.CreateEntry<bool>("UnlimitedAmmo", false);
        protected override void OnLoad()
        {
            string name = Random.Range(0, 100) > 90 ? "AmmoLab" : "Ammo Utils";
            Page = UICore.UtilitiesPage.CreatePage(name, OverrideColor.red);
            Page.CreateBool("Unlimited Ammo", Color.white, UnlimitedAmmo.Value, (a) => UnlimitedAmmo.Value = a);
        }
    }
}
