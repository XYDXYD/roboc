using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	public class CreateClanView : ClanSectionViewBase, IChainRoot
	{
		private const int HEIGHT_SINGLE_BUTTON_AREA = 115;

		private bool _defaultAvatarButtonVisibility;

		private bool _customAvatarButtonVisibility;

		[SerializeField]
		private GameObject ChooseCustomAvatarButton;

		[SerializeField]
		private GameObject ChooseDefaultAvatarButton;

		[SerializeField]
		private GameObject AvatarSectionAnchoringRoot;

		[SerializeField]
		private GameObject errorLabel;

		internal override ClanSectionType SectionType => ClanSectionType.CreateClan;

		public GameObject errorLabelTemplate => errorLabel;

		public void SetUploadCustomAvatarButtonVisibilityStatus(bool status)
		{
			ChooseCustomAvatarButton.SetActive(status);
			_customAvatarButtonVisibility = status;
			CheckAnchoring();
		}

		public void SetDefaultAvatarButtonVisibilityStatus(bool status)
		{
			ChooseDefaultAvatarButton.SetActive(status);
			_defaultAvatarButtonVisibility = status;
			CheckAnchoring();
		}

		private void CheckAnchoring()
		{
			UIWidget component = ChooseCustomAvatarButton.GetComponent<UIWidget>();
			UIWidget component2 = ChooseDefaultAvatarButton.GetComponent<UIWidget>();
			UIWidget component3 = AvatarSectionAnchoringRoot.GetComponent<UIWidget>();
			if (_defaultAvatarButtonVisibility && _customAvatarButtonVisibility)
			{
				component3.bottomAnchor.Set(0f, -230f);
				component2.topAnchor.Set(0f, 115f);
				component.bottomAnchor.Set(0f, 115f);
			}
		}
	}
}
