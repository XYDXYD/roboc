using System.Collections;
using System.Collections.Generic;

namespace Avatars
{
	internal class MultiAvatarLoader_Tencent : IMultiAvatarLoader
	{
		public IEnumerator WaitForAllCustomAvatars(List<string> inputAvatarNames, List<AvatarType> desiredAvatarType)
		{
			yield return null;
		}

		public void ForceRequestAvatar(AvatarType avatarType, string avatarName)
		{
		}

		public void RequestAvatar(AvatarType avatarType, string avatarName)
		{
		}
	}
}
