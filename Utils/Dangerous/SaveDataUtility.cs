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
            Page saveDataUtility = UICore.DangerousPage.CreatePage("Save Data",  ColorPlus.GetNextColor(), maxElements: 10);
            // unlcoks every fuckin thing lad
            var unlocks = saveDataUtility.CreatePage("Player Unlocks", ColorPlus.GetNextColor(), maxElements: 10);
            unlocks.CreateConfirmationFunction("Unlock Everything", ColorPlus.GetNextColor(), UnlockEverything);
            unlocks.CreateConfirmationFunction("Lock Everything", ColorPlus.GetNextColor(), LockEverything);
            var gameControl = new GameObject("Bonelab Game Control").AddComponent<Il2CppSLZ.Bonelab.BonelabGameControl>();
            var cheatControls = gameControl.gameObject.AddComponent<Il2CppSLZ.Bonelab.GameControl_CheatButtons>();
            MonoBehaviour.DontDestroyOnLoad(gameControl.gameObject);
            var progession = saveDataUtility.CreatePage("Player Progession", ColorPlus.GetNextColor(), maxElements: 10);
            progession.CreateBool("Beat Game", ColorPlus.GetNextColor(), DataManager.ActiveSave.Progression.BeatGame, (a) => DataManager.ActiveSave.Progression.BeatGame = a);
            progession.CreateBool("Bodylog Enabled", ColorPlus.GetNextColor(), DataManager.ActiveSave.Progression.BodyLogEnabled, (a) => DataManager.ActiveSave.Progression.BodyLogEnabled = a);
            progession.CreateBool("Has Bodylog", ColorPlus.GetNextColor(), DataManager.ActiveSave.Progression.HasBodyLog, (a) => DataManager.ActiveSave.Progression.HasBodyLog = a);
            progession.CreateInt("Menu Resume", ColorPlus.GetNextColor(), DataManager.ActiveSave.Progression.MenuResume, 1, 0, ushort.MaxValue, (a) => DataManager.ActiveSave.Progression.MenuResume = a);
            progession.CreateString("Current Campaign Level", ColorPlus.GetNextColor(), DataManager.ActiveSave.Progression.CurrentCampaignLevel, (a) => DataManager.ActiveSave.Progression.CurrentCampaignLevel = a);
            progession.CreateString("Current Level", ColorPlus.GetNextColor(), DataManager.ActiveSave.Progression.CurrentLevel, (a) => DataManager.ActiveSave.Progression.CurrentLevel = a);
            progession.CreateInt("Progress", ColorPlus.GetNextColor(), gameControl.Progress, 1, 0, ushort.MaxValue, gameControl.SetProgress);
            progession.CreateConfirmationFunction("Set Full Progress", ColorPlus.GetNextColor(), cheatControls.SetFullProgress);
            saveDataUtility.CreateConfirmationFunction("CONFIRM ALL CHANGES", ColorPlus.GetNextColor(), ()=>
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
