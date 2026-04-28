using BoneLib.Notifications;
using System.Reflection;

namespace LabUtils.Developer {
    public static class DevUtils {
        public static void LoadAllValid<T>(Assembly assembly, Action<Type> runOnValid) {
            string fullName = assembly.FullName;
            if (fullName != null && fullName.Contains("System")) {
                return;
            }
            Type[] types = assembly.GetTypes();
            foreach (Type type in types) {
                if ((!type.Name.Contains("Mono") || !type.Name.Contains("Security")) && typeof(T).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface) {
                    try {
                        runOnValid(type);
                    }
                    catch (Exception ex) {
                        Logger.Error(ex.Message);
                    }
                }
            }
        }

        public static byte[] LoadBytesFromAssembly(Assembly assembly, string name) {
            if (!((ReadOnlySpan<string>)assembly.GetManifestResourceNames()).Contains(name)) {
                return null;
            }
            using Stream stream = assembly.GetManifestResourceStream(name);
            using MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public static Assembly LoadAssemblyFromAssembly(Assembly assembly, string name) {
            byte[] array = LoadBytesFromAssembly(assembly, name);
            if (array == null) {
                return null;
            }
            return Assembly.Load(array);
        }
        internal static Notification Notification {
            get; private set;
        } = new();
        public static void Notify(string message, float timer = 0.3f, string title = "LabUtils") {
            Notification = new();
            Notification.ShowTitleOnPopup = true;
            Notification.Title = title;
            Notification.Message = message;
            Notification.PopupLength = timer;
            Notifier.Send(Notification);
        }
    }
}
