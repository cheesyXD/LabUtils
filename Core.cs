using LabUtils.Developer;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(LabUtils.Core), "LabUtils", "1.0.0", "cheesy", null)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]
[assembly: MelonAdditionalDependencies("BoneLib", "LabFusion")]
namespace LabUtils
{
    public class Core : MelonMod
    {
        public static MelonPreferences_Category Preferences;
        public static Core Instance;
        public override void OnInitializeMelon()
        {
            Preferences = MelonPreferences.CreateCategory("Lab-Utils");
            LoggerInstance.Msg("Loaded LabUtils.");
            HarmonyInstance.PatchAll();
            Instance = this;
            UICore.Init();
            Utility.Init();
        }
        private float autoSaveTimer;
        public override void OnLateUpdate()
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer > 120)
            {
                autoSaveTimer = 0;
                Save();
            }
        }

        public static void Save()
        {
            Preferences.SaveToFile();
            MelonPreferences.Save();
        }

        public override void OnApplicationQuit()
        {
            Save();
        }
    }
}