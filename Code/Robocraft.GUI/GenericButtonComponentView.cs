using System;
using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UIButtonAdapter))]
	public class GenericButtonComponentView : GenericComponentViewBase
	{
		private UIButtonAdapter _buttonAdapter;

		private GenericComponentMessage _message;

		private GenericComponentMessage _mouseEnterMessage;

		private GenericComponentMessage _mouseLeaveMessage;

		private bool _disabledState;

		public override void Setup()
		{
			base.Setup();
			_message = new GenericComponentMessage(MessageType.ButtonClicked, base.Controller.Name, string.Empty);
			GetAdapter().Setup();
			UIButtonAdapter adapter = GetAdapter();
			adapter.OnClickHandler = (Action)Delegate.Combine(adapter.OnClickHandler, new Action(HandleClickEvent));
			_disabledState = false;
		}

		private UIButtonAdapter GetAdapter()
		{
			if (_buttonAdapter == null)
			{
				_buttonAdapter = this.GetComponent<UIButtonAdapter>();
			}
			return _buttonAdapter;
		}

		private void HandleClickEvent()
		{
			if (!_disabledState)
			{
				BubbleMessageUp(_message);
			}
		}

		public void Enable()
		{
			_buttonAdapter.Enable();
			_disabledState = false;
		}

		public void Disable()
		{
			_disabledState = true;
			_buttonAdapter.Disable();
		}

		public override void Hide()
		{
			_buttonAdapter.Disable();
			this.get_gameObject().SetActive(false);
		}

		public override void Show()
		{
			if (_disabledState)
			{
				_buttonAdapter.Disable();
			}
			else
			{
				_buttonAdapter.Enable();
			}
			this.get_gameObject().SetActive(true);
		}

		public void TrackMouseOver()
		{
			if (_mouseEnterMessage == null)
			{
				UIButtonAdapter adapter = GetAdapter();
				adapter.OnEnterHandler = (Action)Delegate.Combine(adapter.OnEnterHandler, new Action(OnMouseEnter));
				UIButtonAdapter adapter2 = GetAdapter();
				adapter2.OnLeaveHandler = (Action)Delegate.Combine(adapter2.OnLeaveHandler, new Action(OnMouseLeave));
				_mouseEnterMessage = new GenericComponentMessage(MessageType.MouseEnter, base.Controller.Name, string.Empty);
				_mouseLeaveMessage = new GenericComponentMessage(MessageType.MouseLeave, base.Controller.Name, string.Empty);
			}
		}

		private void OnMouseEnter()
		{
			if (_mouseEnterMessage != null && !_disabledState)
			{
				BubbleMessageUp(_mouseEnterMessage);
			}
		}

		private void OnMouseLeave()
		{
			if (_mouseLeaveMessage != null && !_disabledState)
			{
				BubbleMessageUp(_mouseLeaveMessage);
			}
		}
	}
}
