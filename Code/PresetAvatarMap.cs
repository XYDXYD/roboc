using System;
using UnityEngine;

internal class PresetAvatarMap : ScriptableObject
{
	[Serializable]
	internal class PresetAvatar
	{
		public Texture2D Texture;
	}

	internal static int MAX_PRESET_AVATAR_COUNT = 15;

	[SerializeField]
	public PresetAvatar[] Avatars = new PresetAvatar[MAX_PRESET_AVATAR_COUNT];

	public PresetAvatarMap()
		: this()
	{
	}

	public Texture2D GetPresetAvatar(int avatarId)
	{
		return Avatars[avatarId].Texture;
	}
}
