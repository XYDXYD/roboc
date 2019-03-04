using Svelto.IoC;

namespace Mothership
{
	internal class IgnoreListMothership : IgnoreList
	{
		[Inject]
		internal GenericInfoDisplay GenericInfoDisplay
		{
			private get;
			set;
		}

		protected override void BlockFriend(string user)
		{
			GenericErrorData data = new GenericErrorData(Localization.Get("strBlockFriendHeader", true), Localization.Get("strBlockFriendBody", true), Localization.Get("strOK", true), Localization.Get("strCancel", true), delegate
			{
				RemoveAndBlockFriend(user);
			}, delegate
			{
			});
			GenericInfoDisplay.ShowInfoDialogue(data);
		}
	}
}
