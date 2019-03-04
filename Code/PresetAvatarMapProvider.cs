using UnityEngine;

internal class PresetAvatarMapProvider
{
	private PresetAvatarMap _avatarMap;

	public PresetAvatarMap GetAvatarMap()
	{
		if (_avatarMap == null)
		{
			_avatarMap = (Resources.Load("PresetAvatarMap") as PresetAvatarMap);
		}
		return _avatarMap;
	}

	public Texture2D GetPresetAvatar(int avatarId)
	{
		return GetAvatarMap().GetPresetAvatar(avatarId);
	}
}
