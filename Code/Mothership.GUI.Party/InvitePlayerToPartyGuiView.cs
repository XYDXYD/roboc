using Robocraft.GUI.Iteration2;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI.Party
{
	internal class InvitePlayerToPartyGuiView : MonoBehaviour, IChainListener
	{
		[SerializeField]
		private GameObject sendInviteButtonTemplate;

		[SerializeField]
		private GameObject playerNameTextFieldTemplate;

		[SerializeField]
		private GameObject _playerListContainer;

		[SerializeField]
		private GameObject _playerListItemTemplate;

		[SerializeField]
		private GameObject _playerListHeaderTemplate;

		[SerializeField]
		private UILabel errorLabel;

		[SerializeField]
		private UILabel noErrorLabel;

		[SerializeField]
		private UIWidget windowTail;

		private InvitePlayerToPartyGuiController _controller;

		private Action _updateAction = delegate
		{
		};

		private Action<object> _iChainListenerAction = delegate
		{
		};

		public GameObject sendInviteButton => sendInviteButtonTemplate;

		public GameObject playerNameTextField => playerNameTextFieldTemplate;

		public GameObject playerListContainer => _playerListContainer;

		public GameObject playerListItemTemplate => _playerListItemTemplate;

		public GameObject playerListHeaderTemplate => _playerListHeaderTemplate;

		public InvitePlayerToPartyGuiView()
			: this()
		{
		}

		public void SetController(InvitePlayerToPartyGuiController controller)
		{
			_controller = controller;
			_updateAction = TickController;
			_iChainListenerAction = ForwardMessageToController;
		}

		public void Listen(object message)
		{
			_iChainListenerAction(message);
		}

		public void Show(UIWidget anchor, UIWidget constrainArea = null)
		{
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			windowTail.SetAnchor(anchor.get_gameObject());
			UIWidget component = this.GetComponent<UIWidget>();
			int height = component.get_height();
			this.get_gameObject().SetActive(true);
			component.topAnchor.target = windowTail.get_transform();
			component.topAnchor.absolute = 0;
			component.topAnchor.relative = 0f;
			component.bottomAnchor.target = windowTail.get_transform();
			component.bottomAnchor.absolute = -height;
			component.bottomAnchor.relative = 0f;
			if (constrainArea != null)
			{
				component.leftAnchor.target = constrainArea.get_transform();
				component.leftAnchor.absolute = 0;
				component.leftAnchor.relative = 0f;
				component.rightAnchor.target = constrainArea.get_transform();
				component.rightAnchor.absolute = 0;
				component.rightAnchor.relative = 1f;
			}
			else
			{
				component.get_transform().set_position(windowTail.get_transform().get_position());
				LayoutUtility.KeepWidgetOnScreen(component);
			}
			component.ResetAnchors();
			component.UpdateAnchors();
			windowTail.UpdateAnchors();
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void ShowNoErrorMsg()
		{
			noErrorLabel.get_gameObject().SetActive(true);
			errorLabel.get_gameObject().SetActive(false);
		}

		public void DisplayErrorMsg(string msg)
		{
			noErrorLabel.get_gameObject().SetActive(false);
			errorLabel.set_text(msg);
			errorLabel.get_gameObject().SetActive(true);
		}

		private void Update()
		{
			_updateAction();
		}

		private void TickController()
		{
			_controller.Tick();
		}

		private void ForwardMessageToController(object message)
		{
			_controller.HandleMessage(message);
		}
	}
}
