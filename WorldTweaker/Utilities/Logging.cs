namespace WorldTweaker.Utilities
{
	internal static class Logging
	{
		public static void Log(string message, TLDLoader.Logger.LogLevel logLevel = TLDLoader.Logger.LogLevel.Info) =>
			WorldTweaker.I.Logger.Log(message, logLevel);

		public static void LogDebug(string message)
		{
#if DEBUG
			WorldTweaker.I.Logger.LogDebug(message);
#endif
		}

		public static void LogInfo(string message) =>
			WorldTweaker.I.Logger.LogInfo(message);

		public static void LogWarning(string message) =>
			WorldTweaker.I.Logger.LogWarning(message);

		public static void LogError(string message) =>
			WorldTweaker.I.Logger.LogError(message);

		public static void LogCritical(string message) =>
			WorldTweaker.I.Logger.LogCritical(message);
	}
}
