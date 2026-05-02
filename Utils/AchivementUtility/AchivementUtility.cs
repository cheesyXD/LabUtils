using Il2CppSLZ.Bonelab;
using LabFusion.Utilities;
using LabUtils.Developer;
using LabUtils.Developer.Extensions;
using UnityEngine;
using Page = BoneLib.BoneMenu.Page;

namespace LabUtils.Utils.AchivementUtility
{
    public class AchivementUtility : Utility
    {
        public static Dictionary<string, string> achievements;
        public static Page SelectedAchivement;
        protected override void OnLoad()
        {
            achievements = Achievements.AchievementsDict.ToDictionary();
            var page = UICore.UtilitiesPage.CreatePage("Achivements", ColorPlus.GetNextColor(), maxElements: 10);
            var achivements = page.CreatePage("Achivements", ColorPlus.GetNextColor(), maxElements:10);
            SelectedAchivement = page.CreatePage("Selected Achivement", ColorPlus.GetNextColor(), maxElements: 10);
            foreach (var item in achievements)
            {
                achivements.CreateFunction(item.Value, ColorPlus.GetNextColor(), () =>
                {
                    SelectAchivement(item);
                });
            }
        }

        private void SelectAchivement(KeyValuePair<string, string> item)
        {
            SelectedAchivement.RemoveAll();
            SelectedAchivement.Name = item.Value;
            SelectedAchivement.CreateConfirmationFunction($"Unlock {item.Value}", ColorPlus.GetNextColor(), () => {
                Achievements.Unlock(item.Key);
                DevUtils.Notify($"Unlocked Achivement: {item.Value}");
            }).SetTooltip("CANNOT BE REVERTED");
            DevUtils.Notify($"Selected Achivement: {item.Value}");
        }
    }
}
