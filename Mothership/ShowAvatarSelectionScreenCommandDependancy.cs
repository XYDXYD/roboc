using System;

namespace Mothership
{
	internal class ShowAvatarSelectionScreenCommandDependancy
	{
		public Action<ShowAvatarSelectionScreenCommandCallbackParameters> OnSelectionCallback;

		public readonly string AvatarSelectionScreenTitle;

		public readonly bool LoadLocalPlayerAvatarInfo;

		public readonly bool CustomAvatarCannotBeSelected;

		public ShowAvatarSelectionScreenCommandDependancy(string AvatarSelectionScreenTitle_, bool LoadLocalPlayerAvatarInfo_, Action<ShowAvatarSelectionScreenCommandCallbackParameters> OnSelectionCallback_, bool CustomAvatarCannotBeSelected_)
		{
			AvatarSelectionScreenTitle = AvatarSelectionScreenTitle_;
			OnSelectionCallback = OnSelectionCallback_;
			LoadLocalPlayerAvatarInfo = LoadLocalPlayerAvatarInfo_;
			CustomAvatarCannotBeSelected = CustomAvatarCannotBeSelected_;
		}
	}
}
