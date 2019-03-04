using UnityEngine;

namespace Mothership
{
	internal class CustomAvatarButton : MonoBehaviour
	{
		public enum AvatarButtonState
		{
			NonInteractable,
			CustomAvatarUploadTextShown,
			PremiumRequiredShown,
			AvatarIsSelectable
		}

		[SerializeField]
		private GameObject PremiumNeededGraphics;

		[SerializeField]
		private GameObject UploadCustomAvatarGraphics;

		[SerializeField]
		private GameObject QuestionMarkBackground;

		[SerializeField]
		private UITexture CustomAvatarImage;

		[SerializeField]
		private GameObject CustomAvatarImageContainer;

		[SerializeField]
		private GameObject Highlight;

		[SerializeField]
		private UIGenericButtonBroadcaster ButtonBroadcaster;

		private AvatarButtonState _buttonState;

		public CustomAvatarButton()
			: this()
		{
		}

		public void SetCustomAvatarTetureOnly(Texture2D texture)
		{
			CustomAvatarImage.set_mainTexture(texture);
		}

		public void SetSelected(bool selectionState)
		{
			Highlight.SetActive(selectionState);
		}

		public void SetState(AvatarButtonState newState)
		{
			_buttonState = newState;
			switch (newState)
			{
			case AvatarButtonState.AvatarIsSelectable:
				UploadCustomAvatarGraphics.SetActive(false);
				PremiumNeededGraphics.SetActive(false);
				QuestionMarkBackground.SetActive(false);
				CustomAvatarImageContainer.SetActive(true);
				ButtonBroadcaster.buttonType = ButtonType.SelectCustomAvatar;
				break;
			case AvatarButtonState.NonInteractable:
				UploadCustomAvatarGraphics.SetActive(false);
				PremiumNeededGraphics.SetActive(false);
				QuestionMarkBackground.SetActive(true);
				CustomAvatarImageContainer.SetActive(false);
				ButtonBroadcaster.buttonType = ButtonType.IgnoreMessage;
				break;
			case AvatarButtonState.CustomAvatarUploadTextShown:
				UploadCustomAvatarGraphics.SetActive(true);
				PremiumNeededGraphics.SetActive(false);
				QuestionMarkBackground.SetActive(true);
				CustomAvatarImageContainer.SetActive(false);
				ButtonBroadcaster.buttonType = ButtonType.Upload;
				break;
			case AvatarButtonState.PremiumRequiredShown:
				UploadCustomAvatarGraphics.SetActive(false);
				PremiumNeededGraphics.SetActive(true);
				QuestionMarkBackground.SetActive(true);
				CustomAvatarImageContainer.SetActive(false);
				ButtonBroadcaster.buttonType = ButtonType.PurchasePremium;
				break;
			}
		}
	}
}
