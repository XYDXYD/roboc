using UnityEngine;

namespace Mothership
{
	public class ClanInvitesView : ClanSectionViewBase
	{
		[SerializeField]
		protected GameObject InviteesListTemplateGO;

		[SerializeField]
		protected GameObject InviteesListItemTemplateGO;

		[SerializeField]
		protected GameObject InviteesListHeaderItemTemplateGO;

		[SerializeField]
		protected GameObject InviteesListContainerAreaGO;

		[SerializeField]
		protected GameObject DeclineAllGO;

		internal override ClanSectionType SectionType => ClanSectionType.ClanInvites;

		public GameObject InviteesListTemplate => InviteesListTemplateGO;

		public GameObject InviteesListItemTemplate => InviteesListItemTemplateGO;

		public GameObject InviteesListHeaderItemTemplate => InviteesListHeaderItemTemplateGO;

		public GameObject InviteesListContainerArea => InviteesListContainerAreaGO;

		public GameObject DeclineAllButtonTemplate => DeclineAllGO;
	}
}
