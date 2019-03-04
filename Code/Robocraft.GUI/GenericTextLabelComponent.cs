namespace Robocraft.GUI
{
	internal class GenericTextLabelComponent : GenericComponentBase
	{
		private GenericTextLabelComponentView _view;

		private int _dataSourceIndex;

		public override IGenericComponentView View => _view;

		public override void HandleMessage(GenericComponentMessage message)
		{
			switch (message.Message)
			{
			case MessageType.RefreshData:
				if (base.DataSource != null)
				{
					if (base.DataSource.NumberOfDataItemsAvailable(0) != 0 || _dataSourceIndex < -1)
					{
						string text = base.DataSource.QueryData<string>(_dataSourceIndex, 0);
						SetText(text);
					}
					else
					{
						SetText(string.Empty);
					}
				}
				break;
			case MessageType.SetData:
				SetText(message.Data.UnpackData<string>());
				break;
			}
			base.HandleMessage(message);
		}

		private void SetText(string txt)
		{
			_view.SetTextLabel(txt);
		}

		public void SetDataSourceIndex(int dataSourceIndex)
		{
			_dataSourceIndex = dataSourceIndex;
		}

		public override void SetView(IGenericComponentView view)
		{
			_view = (view as GenericTextLabelComponentView);
		}

		protected override void OnDataItemChanged(int index1, int index2)
		{
			if (index1 == _dataSourceIndex)
			{
				SetText(base.DataSource.QueryData<string>(_dataSourceIndex, 0));
			}
		}

		protected override void OnAllDataChanged()
		{
			OnDataItemChanged(_dataSourceIndex, 0);
		}
	}
}
