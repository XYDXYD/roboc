using Robocraft.GUI;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class AvatarSelectionView : MonoBehaviour, IInitialize, IChainListener, IChainRoot
	{
		[SerializeField]
		private DefaultAvatarButton DefaultAvatarButtonTemplate;

		[SerializeField]
		private CustomAvatarButton CustomAvatarButtonTemplate;

		[SerializeField]
		private GameObject ContainerForNewButtons;

		[SerializeField]
		private GameObject TopRegion;

		[SerializeField]
		private GameObject MiddleRegion;

		[SerializeField]
		private GameObject BottomRegionSteam;

		[SerializeField]
		private GameObject BottomRegionTencent;

		[SerializeField]
		private GameObject RealUploadButton;

		[SerializeField]
		private GameObject GetPremiumInsteadOfUploadButton;

		[SerializeField]
		private UILabel TitleLabel;

		private List<DefaultAvatarButton> _builtDefaultButtons = new List<DefaultAvatarButton>();

		private CustomAvatarButton _builtCustomAvatarButton;

		private Texture2D _localPlayerCustomTexture;

		private SignalChain _signalChain;

		[Inject]
		internal AvatarSelectionPresenter Presenter
		{
			private get;
			set;
		}

		public AvatarSelectionView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			Presenter.RegisterView(this);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
			TopRegion.SetActive(false);
			MiddleRegion.SetActive(false);
			BottomRegionSteam.SetActive(false);
			BottomRegionTencent.SetActive(true);
		}

		public void SetTitle(string AvatarSelectionScreenTitle)
		{
			TitleLabel.set_text(AvatarSelectionScreenTitle);
		}

		public void SetRegionVisibility(bool avatarButtonsRegionVisible, bool uploadButtonSectionVisible, bool bottomOKandCancelButtonsVisible)
		{
			TopRegion.SetActive(avatarButtonsRegionVisible);
			MiddleRegion.SetActive(uploadButtonSectionVisible);
			BottomRegionTencent.SetActive(bottomOKandCancelButtonsVisible);
			if (!uploadButtonSectionVisible && bottomOKandCancelButtonsVisible)
			{
				UIWidget component = TopRegion.GetComponent<UIWidget>();
				UIWidget component2 = MiddleRegion.GetComponent<UIWidget>();
				UIWidget component3 = BottomRegionTencent.GetComponent<UIWidget>();
			}
		}

		public void ShowGetPremiumButtonInUploadCustomSection(bool setting)
		{
			if (setting)
			{
				RealUploadButton.SetActive(false);
				GetPremiumInsteadOfUploadButton.SetActive(true);
			}
			else
			{
				RealUploadButton.SetActive(true);
				GetPremiumInsteadOfUploadButton.SetActive(false);
			}
		}

		void IChainListener.Listen(object message)
		{
			Presenter.ReceiveMessage(message);
		}

		public void AddNewDefaultButton(Texture2D avatarTexture, int index)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			DefaultAvatarButton defaultAvatarButton = Object.Instantiate<DefaultAvatarButton>(DefaultAvatarButtonTemplate);
			defaultAvatarButton.get_transform().set_parent(ContainerForNewButtons.get_transform());
			defaultAvatarButton.get_transform().set_localScale(Vector3.get_one());
			defaultAvatarButton.get_transform().set_localPosition(Vector3.get_zero());
			defaultAvatarButton.SetTexture(avatarTexture);
			defaultAvatarButton.set_name("Default Button " + index);
			defaultAvatarButton.SetSelfIndex(index);
			defaultAvatarButton.Initialise();
			defaultAvatarButton.get_gameObject().SetActive(true);
			_builtDefaultButtons.Add(defaultAvatarButton);
		}

		public void AddCustomButton()
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			CustomAvatarButton customAvatarButton = Object.Instantiate<CustomAvatarButton>(CustomAvatarButtonTemplate);
			customAvatarButton.get_transform().set_parent(ContainerForNewButtons.get_transform());
			customAvatarButton.get_transform().set_localScale(Vector3.get_one());
			customAvatarButton.get_transform().set_localPosition(Vector3.get_zero());
			customAvatarButton.set_name("Custom Avatar Button");
			customAvatarButton.get_gameObject().SetActive(true);
			_builtCustomAvatarButton = customAvatarButton;
		}

		public void NewLocalPlayerTextureAvailable(Texture2D localPlayerCustomTexture)
		{
			_localPlayerCustomTexture = localPlayerCustomTexture;
			ApplyLocalPlayerCustomTextureToCustomButton();
		}

		public void ArrangeButtonsAndFinalise()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			ContainerForNewButtons.get_transform().set_localPosition(Vector3.get_zero());
			DynamicScalingUIGrid component = ContainerForNewButtons.GetComponent<DynamicScalingUIGrid>();
			if (component != null)
			{
				component.RequiresRecalculation();
			}
			_signalChain = new SignalChain(this.get_transform());
		}

		public void SetSelectedStateOfCustomButton(bool selected)
		{
			if (_builtCustomAvatarButton != null)
			{
				_builtCustomAvatarButton.SetSelected(selected);
			}
		}

		public void SetCustomButtonState(CustomAvatarButton.AvatarButtonState state)
		{
			if (_builtCustomAvatarButton != null)
			{
				_builtCustomAvatarButton.SetState(state);
				if (state != 0)
				{
					ApplyLocalPlayerCustomTextureToCustomButton();
				}
			}
		}

		private void ApplyLocalPlayerCustomTextureToCustomButton()
		{
			if (_builtCustomAvatarButton != null)
			{
				_builtCustomAvatarButton.SetCustomAvatarTetureOnly(_localPlayerCustomTexture);
			}
		}

		public void EraseAllBuiltButtons()
		{
			if (_builtCustomAvatarButton != null)
			{
				_builtCustomAvatarButton.get_transform().set_parent(null);
				Object.Destroy(_builtCustomAvatarButton.get_gameObject());
				_builtCustomAvatarButton = null;
			}
			foreach (DefaultAvatarButton builtDefaultButton in _builtDefaultButtons)
			{
				builtDefaultButton.get_transform().set_parent(null);
				Object.Destroy(builtDefaultButton.get_gameObject());
			}
			_builtDefaultButtons.Clear();
		}

		public void BroadcastMessage(object message)
		{
			_signalChain.Broadcast<object>(message);
		}
	}
}
