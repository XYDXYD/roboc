using UnityEngine;

namespace Mothership
{
	public class FriendListView : FriendSectionViewBase
	{
		[SerializeField]
		protected GameObject addFriendButtonGO;

		[SerializeField]
		protected GameObject FriendListTemplateGO;

		[SerializeField]
		protected GameObject FriendListItemTemplateGO;

		[SerializeField]
		protected GameObject FriendListContainerAreaGO;

		[SerializeField]
		protected GameObject FriendListHeaderItemTemplateGO;

		internal override FriendSectionType SectionType => FriendSectionType.FriendList;

		public GameObject AddFriendButton => addFriendButtonGO;

		public GameObject FriendListTemplate => FriendListTemplateGO;

		public GameObject FriendListItemTemplate => FriendListItemTemplateGO;

		public GameObject FriendListContainerArea => FriendListContainerAreaGO;

		public GameObject FriendListHeaderItemTemplate => FriendListHeaderItemTemplateGO;
	}
}
