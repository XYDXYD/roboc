namespace Robocraft.GUI
{
	internal class GenericTickBoxComponent : GenericComponentBase
	{
		private GenericTickBoxComponentView _view;

		private int _dataSourceIndex;

		public override IGenericComponentView View => _view;

		public override void HandleMessage(GenericComponentMessage message)
		{
			if (message.Message == MessageType.Disable)
			{
				_view.DisableSelf();
			}
			if (message.Message == MessageType.Enable)
			{
				_view.EnableSelf();
			}
			if (message.Message == MessageType.RefreshData && base.DataSource != null)
			{
				if (base.DataSource.NumberOfDataItemsAvailable(0) != 0)
				{
					bool tickBoxState = base.DataSource.QueryData<bool>(_dataSourceIndex, 0);
					_view.SetTickBoxState(tickBoxState);
				}
				else
				{
					_view.SetTickBoxState(newState: false);
				}
			}
			base.HandleMessage(message);
		}

		internal void SetDataSourceIndex(int dataSourceIndex)
		{
			_dataSourceIndex = dataSourceIndex;
		}

		public void HandleTickBoxChanged(bool newSetting)
		{
			if (newSetting)
			{
				_view.BubbleMessageUp(new GenericComponentMessage(MessageType.TickBoxSet, base.Name, string.Empty));
			}
			else
			{
				_view.BubbleMessageUp(new GenericComponentMessage(MessageType.TickBoxCleared, base.Name, string.Empty));
			}
		}

		public override void SetView(IGenericComponentView view)
		{
			_view = (view as GenericTickBoxComponentView);
		}
	}
}
