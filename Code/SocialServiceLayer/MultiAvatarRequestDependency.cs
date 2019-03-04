using Avatars;

namespace SocialServiceLayer
{
	internal struct MultiAvatarRequestDependency
	{
		public readonly string name;

		public readonly AvatarType avatarType;

		public MultiAvatarRequestDependency(string name_, AvatarType avatarType_)
		{
			name = name_;
			avatarType = avatarType_;
		}
	}
}
