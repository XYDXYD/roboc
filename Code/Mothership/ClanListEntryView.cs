using Robocraft.GUI;
using SocialServiceLayer;
using UnityEngine;

namespace Mothership
{
	public class ClanListEntryView : GenericListEntryViewBase
	{
		[SerializeField]
		private UILabel ClanName;

		[SerializeField]
		private UILabel ClanStatus;

		[SerializeField]
		private UILabel ClanMemberCount;

		[SerializeField]
		private UITexture ClanTexture;

		private const int MAX_CLAN_MEMBERS = 50;

		public override void Default()
		{
			base.Default();
			ClanName.set_text(string.Empty);
			ClanStatus.set_text(string.Empty);
			ClanMemberCount.set_text("0 / 0");
		}

		public override void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if (genericComponentMessage.Target == string.Empty && genericComponentMessage.Message == MessageType.ButtonClicked)
				{
					genericComponentMessage.Consume();
					BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, genericComponentMessage.Originator, new ClanListItemComponentDataContainer(this, ClanName.get_text())));
				}
			}
		}

		public override void UpdateData(object data)
		{
			ClanPlusAvatarInfo clanPlusAvatarInfo = data as ClanPlusAvatarInfo;
			ClanName.set_text(clanPlusAvatarInfo.clanName);
			switch (clanPlusAvatarInfo.ClanType)
			{
			case ClanType.Open:
				ClanStatus.set_text(StringTableBase<StringTable>.Instance.GetString("strClanTypeOpen"));
				break;
			case ClanType.Closed:
				ClanStatus.set_text(StringTableBase<StringTable>.Instance.GetString("strClanTypeClosed"));
				break;
			}
			ClanMemberCount.set_text(FastConcatUtility.FastConcat<string>(FastConcatUtility.FastConcat<string>(clanPlusAvatarInfo.clanSize.ToString(), " / "), 50.ToString()));
			if (clanPlusAvatarInfo.avatarTexture == null)
			{
				ClanTexture.set_mainTexture(AvatarUtils.StillLoadingTexture);
			}
			else
			{
				ClanTexture.set_mainTexture(clanPlusAvatarInfo.avatarTexture);
			}
		}
	}
}
