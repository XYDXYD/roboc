using UnityEngine;

namespace Mothership
{
	public class YourClanView : ClanSectionViewBase
	{
		[SerializeField]
		protected GameObject yourclanDescriptionTextLabelTemplateGO;

		[SerializeField]
		protected GameObject yourclanDescriptionTooltipTextLabelTemplateGO;

		[SerializeField]
		protected GameObject yourclanDescriptionTextEntryTemplateGO;

		[SerializeField]
		protected GameObject yourclanNameTextLabelTemplateGO;

		[SerializeField]
		protected GameObject yourclanInviteStatusTemplateGO;

		[SerializeField]
		protected GameObject yourclanInviteStatusPopUpListTemplateGO;

		[SerializeField]
		protected GameObject yourclanAvatarImageTemplateGO;

		[SerializeField]
		protected GameObject yourclanUploadAvatarImageButtonTemplateGO;

		[SerializeField]
		protected GameObject PlayerListTemplateGO;

		[SerializeField]
		protected GameObject PlayerListItemTemplateGO;

		[SerializeField]
		protected GameObject PlayerListHeaderItemTemplateGO;

		[SerializeField]
		protected GameObject PlayerListContainerAreaGO;

		[SerializeField]
		protected GameObject LessThanTenMembersWarningMessageGO;

		[SerializeField]
		protected GameObject LeaveButtonTemplateGO;

		[SerializeField]
		protected GameObject InviteButtonTemplateGO;

		[SerializeField]
		protected GameObject JoinButtonTemplateGO;

		[SerializeField]
		protected GameObject seasonRobitsLabelTemplateGO;

		[SerializeField]
		protected GameObject seasonExperienceLabelTemplateGO;

		[SerializeField]
		protected GameObject clanSizeLabelTemplateGO;

		[SerializeField]
		protected GameObject ErrorLabelTemplateGO;

		internal override ClanSectionType SectionType => ClanSectionType.YourClan;

		public GameObject yourclanDescriptionTextLabelTemplate => yourclanDescriptionTextLabelTemplateGO;

		public GameObject yourclanDescriptionTooltipTextLabelTemplate => yourclanDescriptionTooltipTextLabelTemplateGO;

		public GameObject yourclanDescriptionTextEntryTemplate => yourclanDescriptionTextEntryTemplateGO;

		public GameObject yourclanNameTextLabelTemplate => yourclanNameTextLabelTemplateGO;

		public GameObject yourclanInviteStatusTemplate => yourclanInviteStatusTemplateGO;

		public GameObject yourclanInviteStatusPopUpListTemplate => yourclanInviteStatusPopUpListTemplateGO;

		public GameObject yourclanAvatarImageTemplate => yourclanAvatarImageTemplateGO;

		public GameObject yourclanUploadAvatarImageButtonTemplate => yourclanUploadAvatarImageButtonTemplateGO;

		public GameObject InviteButtonTemplate => InviteButtonTemplateGO;

		public GameObject LeaveButtonTemplate => LeaveButtonTemplateGO;

		public GameObject JoinButtonTemplate => JoinButtonTemplateGO;

		public GameObject PlayerListTemplate => PlayerListTemplateGO;

		public GameObject PlayerListItemTemplate => PlayerListItemTemplateGO;

		public GameObject PlayerListHeaderItemTemplate => PlayerListHeaderItemTemplateGO;

		public GameObject PlayerListContainerArea => PlayerListContainerAreaGO;

		public GameObject LessThanTenMembersWarningMessage => LessThanTenMembersWarningMessageGO;

		public GameObject seasonRobitsLabelTemplate => seasonRobitsLabelTemplateGO;

		public GameObject seasonExperienceLabelTemplate => seasonExperienceLabelTemplateGO;

		public GameObject clanSizeLabelTemplate => clanSizeLabelTemplateGO;

		public GameObject ErrorLabelTemplate => ErrorLabelTemplateGO;
	}
}
