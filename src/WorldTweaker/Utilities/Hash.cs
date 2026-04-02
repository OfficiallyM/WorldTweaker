namespace WorldTweaker.Utilities
{
	internal static class Hash
	{
		public static int FNV1(string str)
		{
			unchecked
			{
				int hash = (int)2166136261;
				foreach (char c in str)
				{
					hash ^= c;
					hash *= 16777619;
				}
				return hash;
			}
		}
	}
}
