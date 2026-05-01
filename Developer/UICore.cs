using BoneLib.BoneMenu;
using UnityEngine;

namespace LabUtils.Developer {
    public static class UICore {
        public static Page RootPage { get; private set; }
        public static Page UtilitiesPage {
            get; private set;
        }
        public static Page DangerousPage {
            get; private set;
        }
        public static Page SettingsPage {
            get; private set;
        }
        internal static void Init() {
            RootPage = Page.Root.CreatePage("LabUtils", OverrideColor.lightBlue, maxElements: 10);
            RootPage.CreateFunction("Save Preferences", Color.green, Core.Save);
            UtilitiesPage = RootPage.CreatePage("Utilities", OverrideColor.lightBlue, maxElements: 10);
            DangerousPage = RootPage.CreatePage("Dangerous", OverrideColor.red, maxElements: 10);
            SettingsPage = RootPage.CreatePage("Settings", Color.yellow, maxElements: 10);
        }

        public static FunctionElement CreateConfirmationFunction(Page page, string text, Color color, Action onConfirm) {
            var confirmation = page.CreatePage(text, color, maxElements: 10);
            var confirm = confirmation.CreateFunction("Confirm", OverrideColor.green, ()=> { onConfirm?.Invoke(); DevUtils.Notify($"Confirmed Choice for {text}!", 0.5f, "LabUtils"); });
            var link = confirmation.CreatePageLink(page);
            link.ElementColor = OverrideColor.red;
            link.ElementName = "Cancel";
            return confirm;
        }
    }
}
