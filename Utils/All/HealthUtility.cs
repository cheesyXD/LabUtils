using BoneLib;
using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow;
using LabUtils.Developer;
using MelonLoader;
using UnityEngine;
namespace LabUtils.Utils.All
{
    public class HealthUtility : Utility
    {
        public static Page Page;
        public static MelonPreferences_Entry<bool> GodMode { get; set; } = Core.Preferences.CreateEntry("IDDQD ", false);
        public static MelonPreferences_Entry<DeathSettings> ReloadOnDeath { get; set; } = Core.Preferences.CreateEntry("ReloadOnDeath ", DeathSettings.Default);
        public enum DeathSettings : byte
        {
            Default,
            On,
            Off,
        }
        public Health.HealthMode HealthMode { get
            {
                return !Player.RigManager ? Health.HealthMode.Mortal : Player.RigManager.health.healthMode;
            } 
        set
            {
                if (!Player.RigManager) return;
                Player.RigManager.health.SetHealthMode((int)value);
            }
        }
        protected override void OnLoad()
        {
            Hooking.OnLevelLoaded += Hooking_OnLevelLoaded;
            Page = UICore.UtilitiesPage.CreatePage("Health Utility", OverrideColor.green);
            Page.CreateBool("God Mode", Color.white, GodMode.Value, ChangeGodMode);
            Page.CreateFunction("Heal", Color.white, Heal);
            Page.CreateFunction("DIE", Color.white, Die);
            Page.CreateEnum("Health Mode", Color.white, HealthMode, ChangeHealthMode);
            Page.CreateEnum("Reload Level On Death", Color.white, ReloadOnDeath.Value , ReloadOnDeathMode);
        }
        private static bool defaultReloadOnDeath;
        private void Hooking_OnLevelLoaded(LevelInfo obj)
        {
            var decor = Player.RigManager.health as Player_Health;
            if (decor == null) return;
            defaultReloadOnDeath = decor.reloadLevelOnDeath;
            ReloadOnDeathMode(ReloadOnDeath.Value);
        }

        private void ReloadOnDeathMode(Enum @enum)
        {
            ReloadOnDeath.Value = (DeathSettings)@enum;
            var decor = Player.RigManager.health as Player_Health;
            if (decor == null) return;
            switch (ReloadOnDeath.Value)
            {
                case DeathSettings.Default:
                    decor.reloadLevelOnDeath = defaultReloadOnDeath;
                    break;
                case DeathSettings.On:
                    decor.reloadLevelOnDeath = true;
                    break; 
                case DeathSettings.Off:
                    decor.reloadLevelOnDeath = false;
                    break;
            }
        }

        private void ChangeHealthMode(Enum @enum)
        {
            HealthMode = (Health.HealthMode)@enum;
        }

        private void Die()
        {
            if (System.Random.Shared.Next(0, 3) > 3)
            {
                DevUtils.Notify("Go tell aunt rhody");
            }
            Player.RigManager.health.curr_Health = 0;
            Player.RigManager.health.TAKEDAMAGE(ushort.MaxValue);
        }

        private void Heal()
        {
            Player.RigManager.health.SetFullHealth();
        }

        private void ChangeGodMode(bool obj)
        {
            GodMode.Value = obj;
            DevUtils.Notify(obj ? "Degreelessness Mode On" : "Degreelessness Mode Off", 1f);
        }
    }
}
