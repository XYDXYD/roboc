using System;

namespace Robocraft.GUI
{
	public class GenericTickBoxComponentView : GenericComponentViewBase
	{
		private UITickBoxAdapter _tickBox;

		public override void Setup()
		{
			base.Setup();
			_tickBox = this.GetComponentInChildren<UITickBoxAdapter>();
			_tickBox.Setup();
			UITickBoxAdapter tickBox = _tickBox;
			tickBox.OnTickedStateChanged = (Action<bool>)Delegate.Combine(tickBox.OnTickedStateChanged, new Action<bool>(OnTickBoxStateChanged));
		}

		public override void Listen(object message)
		{
			base.Listen(message);
		}

		public override void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public override void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void EnableSelf()
		{
			_tickBox.Enable();
		}

		public void DisableSelf()
		{
			_tickBox.Disable();
		}

		internal void OnTickBoxStateChanged(bool newState)
		{
			(_controller as GenericTickBoxComponent).HandleTickBoxChanged(newState);
		}

		internal void SetTickBoxState(bool newState)
		{
			_tickBox.SetTickState(newState);
		}
	}
}
