using MelonLoader;

namespace LabUtils.Developer {
    public static class Logger {
        public static void Log(object message) => MelonLogger.Msg(message);
        public static void Warn(object message) => MelonLogger.Warning(message);
        public static void Error(object message, System.Exception exception = null) => MelonLogger.Error(message.ToString(), exception);
    }
}
