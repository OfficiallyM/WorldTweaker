using System.IO;
using TLDLoader;
using UnityEngine;

namespace WorldTweaker.Utilities
{
	internal static class TextureUtilities
	{
		public static Texture2D LoadTexture(string file)
		{
			string path = Path.Combine(ModLoader.GetModAssetsFolder(WorldTweaker.I), file);
			if (!File.Exists(path))
			{
				Logging.LogError($"File not found: {path}.");
				return null;
			}

			byte[] fileData = File.ReadAllBytes(path);
			Texture2D texture = new Texture2D(2, 2); 

			if (texture.LoadImage(fileData))
				return texture;

			Logging.LogError($"Failed to load texture from: {path}.");
			return null;
		}
	}
}
