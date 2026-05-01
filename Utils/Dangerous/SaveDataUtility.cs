using BoneLib.BoneMenu;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow.Warehouse;
using LabUtils.Developer;
using UnityEngine;

namespace LabUtils.Utils.Dangerous
{
    internal class SaveDataUtility : Utility
    {
        public float percentage;
        protected override void OnLoad()
        {
            Page saveDataUtility = UICore.DangerousPage.CreatePage("Save Data", OverrideColor.red, maxElements: 10);
            // unlcoks every fuckin thing lad
            var unlocks = saveDataUtility.CreatePage("Player Unlocks", Color.white, maxElements: 10);
            UICore.CreateConfirmationFunction(unlocks, "Unlock Everything", Color.white, UnlockEverything);
            UICore.CreateConfirmationFunction(unlocks, "Lock Everything", Color.white, LockEverything);
            var gameControl = new GameObject("Bonelab Game Control").AddComponent<Il2CppSLZ.Bonelab.BonelabGameControl>();
            var cheatControls = gameControl.gameObject.AddComponent<Il2CppSLZ.Bonelab.GameControl_CheatButtons>();
            MonoBehaviour.DontDestroyOnLoad(gameControl.gameObject);
            var progession = saveDataUtility.CreatePage("Player Progession", Color.white, maxElements: 10);
            progession.CreateBool("Beat Game", Color.white, DataManager.ActiveSave.Progression.BeatGame, (a) => DataManager.ActiveSave.Progression.BeatGame = a);
            progession.CreateBool("Bodylog Enabled", Color.white, DataManager.ActiveSave.Progression.BodyLogEnabled, (a) => DataManager.ActiveSave.Progression.BodyLogEnabled = a);
            progession.CreateBool("Has Bodylog", Color.white, DataManager.ActiveSave.Progression.HasBodyLog, (a) => DataManager.ActiveSave.Progression.HasBodyLog = a);
            progession.CreateInt("Menu Resume", Color.white, DataManager.ActiveSave.Progression.MenuResume, 1, 0, ushort.MaxValue, (a) => DataManager.ActiveSave.Progression.MenuResume = a);
            progession.CreateString("Current Campaign Level", Color.white, DataManager.ActiveSave.Progression.CurrentCampaignLevel, (a) => DataManager.ActiveSave.Progression.CurrentCampaignLevel = a);
            progession.CreateString("Current Level", Color.white, DataManager.ActiveSave.Progression.CurrentLevel, (a) => DataManager.ActiveSave.Progression.CurrentLevel = a);
            progession.CreateInt("Progress", Color.white, gameControl.Progress, 1, 0, ushort.MaxValue, gameControl.SetProgress);
            UICore.CreateConfirmationFunction(progession, "Set Full Progress", Color.white, cheatControls.SetFullProgress);
            UICore.CreateConfirmationFunction(saveDataUtility, "CONFIRM ALL CHANGES", Color.white, ()=>
            {
                gameControl.JustSave();
                DataManager.TrySaveActiveSave(Il2CppSLZ.Marrow.SaveData.SaveFlags.DefaultAndPlayerSettingsAndUnlocks | Il2CppSLZ.Marrow.SaveData.SaveFlags.Progression | Il2CppSLZ.Marrow.SaveData.SaveFlags.Complete);
            });
        }

        private void UnlockEverything()
        {
            foreach (var crate in AssetWarehouse.Instance.GetCrates())
            {
                DataManager.ActiveSave.Unlocks.IncrementUnlockForBarcode(crate.Barcode);
            }
        }

        private void LockEverything()
        {
            foreach (var crate in AssetWarehouse.Instance.GetCrates())
            {
                DataManager.ActiveSave.Unlocks.ClearUnlockForBarcode(crate.Barcode);
            }
        }
    }
}
