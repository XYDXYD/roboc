using Mothership;

namespace Robocraft.GUI
{
	internal class GenericTextEntryComponent : GenericComponentBase
	{
		private GenericTextEntryComponentView _view;

		private int _dataSourceIndex;

		public override IGenericComponentView View => _view;

		public override void HandleMessage(GenericComponentMessage message)
		{
			if (message.Message == MessageType.RefreshData)
			{
				if (base.DataSource != null)
				{
					if (base.DataSource.NumberOfDataItemsAvailable(0) != 0)
					{
						string textEntryStartValue = base.DataSource.QueryData<string>(_dataSourceIndex, 0);
						_view.SetTextEntryStartValue(textEntryStartValue);
					}
					else
					{
						_view.SetTextEntryStartValue(string.Empty);
					}
				}
			}
			else if (message.Message == MessageType.SetFocus)
			{
				_view.ChangeTextEntryFocus(focus: true);
			}
			else if (message.Message == MessageType.SetUnfocus)
			{
				_view.ChangeTextEntryFocus(focus: false);
			}
			base.HandleMessage(message);
		}

		internal void SetDataSourceIndex(int dataSourceIndex)
		{
			_dataSourceIndex = dataSourceIndex;
		}

		public void HandleTextSubmitted(string textSubmitted)
		{
			_view.BubbleMessageUp(new GenericComponentMessage(MessageType.TextEntrySubmitted, string.Empty, base.Name, new TextEntryComponentDataContainer(textSubmitted)));
		}

		internal void HandleInputGetFocus()
		{
			_view.BubbleMessageUp(new GenericComponentMessage(MessageType.OnFocus, base.Name, string.Empty));
		}

		internal void HandleInputLoseFocus()
		{
			_view.BubbleMessageUp(new GenericComponentMessage(MessageType.OnUnfocus, base.Name, string.Empty));
		}

		public void HandleTextChanged(string newText)
		{
			_view.BubbleMessageUp(new GenericComponentMessage(MessageType.TextEntryChanged, string.Empty, base.Name, new TextEntryComponentDataContainer(newText)));
		}

		public override void SetView(IGenericComponentView view)
		{
			_view = (view as GenericTextEntryComponentView);
		}

		public void HandleSocialMessage(SocialMessage socialMessage)
		{
			if (socialMessage.messageDispatched == SocialMessageType.ClanScreenClosed)
			{
				_view.ChangeTextEntryFocus(focus: false);
			}
		}
	}
}
