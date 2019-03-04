namespace Robocraft.GUI
{
	internal class GenericButtonComponent : GenericComponentBase
	{
		protected GenericButtonComponentView _view;

		public override IGenericComponentView View => _view;

		public override void SetView(IGenericComponentView view)
		{
			_view = (view as GenericButtonComponentView);
		}

		public override void HandleMessage(GenericComponentMessage message)
		{
			switch (message.Message)
			{
			case MessageType.Enable:
				_view.Enable();
				break;
			case MessageType.Disable:
				_view.Disable();
				break;
			}
			base.HandleMessage(message);
		}
	}
}
