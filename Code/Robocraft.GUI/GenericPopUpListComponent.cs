using SocialServiceLayer;

namespace Robocraft.GUI
{
	internal sealed class GenericPopUpListComponent : GenericComponentBase
	{
		private GenericPopUpListComponentView _view;

		private PopUpListComponentDataContainer _dataContainer = new PopUpListComponentDataContainer(ClanType.Open);

		private int _dataSourceIndex1;

		private int _dataSourceIndex2;

		public override IGenericComponentView View => _view;

		public override void HandleMessage(GenericComponentMessage message)
		{
			if (message.Message == MessageType.RefreshData && base.DataSource != null && base.DataSource.NumberOfDataItemsAvailable(0) != 0)
			{
				string currentSelection = base.DataSource.QueryData<string>(_dataSourceIndex1, _dataSourceIndex2);
				_view.SetCurrentSelection(currentSelection);
			}
			base.HandleMessage(message);
		}

		internal void SetDataSourceIndices(int dataSourceIndex1, int dataSourceIndex2)
		{
			_dataSourceIndex1 = dataSourceIndex1;
			_dataSourceIndex2 = dataSourceIndex2;
		}

		public void HandleValueChanged(int valueIndex)
		{
			_dataContainer.PackData((ClanType)valueIndex);
			_view.BubbleMessageUp(new GenericComponentMessage(MessageType.PopUpListChanged, string.Empty, base.Name, _dataContainer));
		}

		public override void SetView(IGenericComponentView view)
		{
			_view = (view as GenericPopUpListComponentView);
		}
	}
}
