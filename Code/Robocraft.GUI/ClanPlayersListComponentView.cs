using UnityEngine;

namespace Robocraft.GUI
{
	public class ClanPlayersListComponentView : GenericExpandeableListComponentView
	{
		private GameObject _lessThanTenMembersMessage;

		private AnchorPoint _originalAnchoring;

		public override void Show()
		{
			base.Show();
		}

		public override void Hide()
		{
			base.Hide();
		}

		public void ShowLessThanTenMembers(bool showLessThanTenMembersWarning)
		{
			UIWidget component = _lessThanTenMembersMessage.GetComponent<UIWidget>();
			if (showLessThanTenMembersWarning)
			{
				component.bottomAnchor = _originalAnchoring;
			}
			else
			{
				component.bottomAnchor = component.topAnchor;
			}
			_lessThanTenMembersMessage.SetActive(showLessThanTenMembersWarning);
		}

		public void SetWarningMessageGO(GameObject warningMessage)
		{
			_lessThanTenMembersMessage = warningMessage;
			UIWidget component = _lessThanTenMembersMessage.GetComponent<UIWidget>();
			_originalAnchoring = component.bottomAnchor;
		}
	}
}
