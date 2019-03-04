using Authentication;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal static class AvatarUtils
{
	private static Texture2D _failedIcon;

	private static Texture2D _stillLoadingIcon;

	public static Texture2D FailedToLoadTexture
	{
		get
		{
			if (_failedIcon == null)
			{
				_failedIcon = Resources.Load<Texture2D>("FailedToLoad");
			}
			return _failedIcon;
		}
	}

	public static Texture2D StillLoadingTexture
	{
		get
		{
			if (_stillLoadingIcon == null)
			{
				_stillLoadingIcon = Resources.Load<Texture2D>("Downloading");
			}
			return _stillLoadingIcon;
		}
	}

	public static string LocalPlayerAvatarName => User.Username;

	public static Texture2D DeserialiseAvatar(CustomAvatarInfo customAvatarInfo)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		if (customAvatarInfo.CustomAvatarBytes == null)
		{
			return null;
		}
		Texture2D val = new Texture2D(100, 100, 10, false);
		try
		{
			ImageConversion.LoadImage(val, customAvatarInfo.CustomAvatarBytes);
			return val;
		}
		catch (Exception arg)
		{
			Console.LogError("Failed to deserialise avatar image. Exception thrown: " + arg);
			return null;
		}
	}

	public static void CreateAvatarAtlas(Dictionary<string, Texture2D> avatars, out Texture2D atlasTexture, out ReadOnlyDictionary<string, Rect> atlasRects)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, Rect> dictionary = new Dictionary<string, Rect>();
		atlasTexture = new Texture2D(1, 1, 10, false);
		Rect[] array = atlasTexture.PackTextures(avatars.Values.ToArray(), 10);
		using (Dictionary<string, Texture2D>.Enumerator enumerator = avatars.GetEnumerator())
		{
			int num = 0;
			while (enumerator.MoveNext())
			{
				dictionary.Add(enumerator.Current.Key, array[num++]);
			}
		}
		atlasRects._002Ector(dictionary);
	}

	public static int ChooseAvatarIdForAi(string aiName)
	{
		return Math.Abs(aiName.GetHashCode()) % PresetAvatarMap.MAX_PRESET_AVATAR_COUNT;
	}
}
