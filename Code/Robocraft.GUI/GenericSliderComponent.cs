namespace Robocraft.GUI
{
	internal class GenericSliderComponent : GenericComponentBase
	{
		private GenericSliderComponentView _view;

		private int _dataSourceIndex;

		public override IGenericComponentView View => _view;

		public override void HandleMessage(GenericComponentMessage message)
		{
			switch (message.Message)
			{
			case MessageType.Show:
				_view.Show();
				break;
			case MessageType.Hide:
				_view.Hide();
				break;
			case MessageType.Disable:
				_view.DisableSelf();
				break;
			case MessageType.Enable:
				_view.EnableSelf();
				break;
			case MessageType.RefreshData:
				if (base.DataSource != null)
				{
					if (base.DataSource.NumberOfDataItemsAvailable(0) != 0)
					{
						float sliderValue = base.DataSource.QueryData<float>(_dataSourceIndex, 0);
						SetSliderValue(sliderValue);
					}
					else
					{
						SetSliderValue(0f);
					}
				}
				break;
			}
			base.HandleMessage(message);
		}

		private void SetSliderValue(float value)
		{
			_view.SetSliderValue(value);
		}

		public void HandleValueChanging(float newValue)
		{
			SliderComponentDataContainer data_ = new SliderComponentDataContainer(newValue);
			_view.BubbleMessageUp(new GenericComponentMessage(MessageType.SliderValueAdjusting, string.Empty, base.Name, data_));
		}

		public void HandleValueConfirmed(float newValue)
		{
			SliderComponentDataContainer data_ = new SliderComponentDataContainer(newValue);
			_view.BubbleMessageUp(new GenericComponentMessage(MessageType.SliderValueConfirmed, string.Empty, base.Name, data_));
		}

		public void SetDataSourceIndex(int dataSourceIndex)
		{
			_dataSourceIndex = dataSourceIndex;
		}

		public override void SetView(IGenericComponentView view)
		{
			_view = (view as GenericSliderComponentView);
		}

		protected override void OnDataItemChanged(int index1, int index2)
		{
			if (index1 == _dataSourceIndex)
			{
				SetSliderValue(base.DataSource.QueryData<float>(_dataSourceIndex, 0));
			}
		}

		protected override void OnAllDataChanged()
		{
			OnDataItemChanged(_dataSourceIndex, 0);
		}
	}
}
