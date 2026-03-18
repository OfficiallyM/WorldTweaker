using TLDLoader;

namespace WorldTweaker.Utilities
{
	internal static class ModLoaderUtilities
	{
		public static bool DoesModExist(string modId)
		{
			foreach (var mod in ModLoader.LoadedMods)
			{
				if (mod.ID == modId)
					return true;
			}
			return false;
		}
	}
}
