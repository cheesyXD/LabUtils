using MelonLoader;
using System.Reflection;

namespace LabUtils.Developer {
    public abstract class Utility {
        public static List<Utility> Utilities = new();
        internal static void Init() {
            Utilities.Clear();
            Logger.Log("Loading DevUtils from Melons");
            foreach (MelonBase registeredMelon in MelonBase.RegisteredMelons) {
                var a = registeredMelon.MelonAssembly.Assembly;
                DevUtils.LoadAllValid<Utility>(registeredMelon.MelonAssembly.Assembly, OnUtilLoad);
            }
        }

        private static void OnUtilLoad(Type type) {
            if (Activator.CreateInstance(type) is not Utility util) {
                return;
            }
                Utilities.Add(util);
            util.Load();
        }

        public virtual void Load() {
            Logger.Log($"Loaded {GetType().FullName} Util");
            OnLoad();
        }
        protected abstract void OnLoad();
    }
}
