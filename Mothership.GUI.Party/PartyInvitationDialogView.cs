using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Party
{
	public class PartyInvitationDialogView : MonoBehaviour, IChainListener, IInitialize
	{
		public UILabel playerNameLabel;

		public UITexture playerAvatarTexture;

		private const string REFERENCE_ANCHOR_TAG = "PartyPanelContainer";

		private BubbleSignal<IPartyGUIViewRoot> _bubble;

		[Inject]
		internal IPartyInvitationDialogController partyInvitationDialogController
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController guiInputController
		{
			private get;
			set;
		}

		public PartyInvitationDialogView()
			: this()
		{
		}

		void IChainListener.Listen(object message)
		{
			partyInvitationDialogController.ReceiveMessage(message);
		}

		public void DispatchMessage(object message)
		{
			_bubble.TargetedDispatch<object>(message);
		}

		public void OnDependenciesInjected()
		{
			partyInvitationDialogController.RegisterView(this);
			_bubble = new BubbleSignal<IPartyGUIViewRoot>(this.get_transform());
			Hide();
			guiInputController.OnScreenStateChange += OnScreenSwitch;
		}

		private void Update()
		{
			partyInvitationDialogController.CheckForHotkeyInput();
		}

		private void OnScreenSwitch()
		{
			GameObject val = GameObject.FindGameObjectWithTag("PartyPanelContainer");
			if (val != null)
			{
				UIWidget component = this.GetComponent<UIWidget>();
				UIWidget component2 = val.GetComponent<UIWidget>();
				component.SetAnchor(component2.get_gameObject(), 0, 0, 0, 0);
			}
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
			OnScreenSwitch();
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void SetInviterName(string playerName)
		{
			playerNameLabel.set_text(playerName);
		}

		public void SetInviterAvatar(Texture2D playerAvatar)
		{
			playerAvatarTexture.set_mainTexture(playerAvatar);
		}
	}
}
