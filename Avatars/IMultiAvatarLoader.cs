using System.Collections;
using System.Collections.Generic;

namespace Avatars
{
	internal interface IMultiAvatarLoader
	{
		IEnumerator WaitForAllCustomAvatars(List<string> inputAvatarNames, List<AvatarType> desiredAvatarType);

		void ForceRequestAvatar(AvatarType avatarType, string avatarName);

		void RequestAvatar(AvatarType avatarType, string avatarName);
	}
}
