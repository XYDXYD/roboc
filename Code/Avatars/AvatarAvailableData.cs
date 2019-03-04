using UnityEngine;

namespace Avatars
{
	internal struct AvatarAvailableData
	{
		public AvatarType avatarType;

		public string avatarName;

		public Texture2D texture;

		public AvatarAvailableData(AvatarType avatarType_, string name_, Texture2D texture_)
		{
			avatarType = avatarType_;
			avatarName = name_;
			texture = texture_;
		}
	}
}
