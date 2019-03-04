using UnityEngine;

namespace Robocraft.GUI
{
	internal class GenericImageComponent : GenericComponentBase
	{
		private GenericImageComponentView _view;

		private int _dataSourceIndex;

		public override IGenericComponentView View => _view;

		public override void HandleMessage(GenericComponentMessage message)
		{
			if (message.Message == MessageType.RefreshData)
			{
				RefreshFromData();
			}
			base.HandleMessage(message);
		}

		private void RefreshFromData()
		{
			if (base.DataSource.NumberOfDataItemsAvailable(0) != 0)
			{
				Texture2D image = base.DataSource.QueryData<Texture2D>(_dataSourceIndex, 0);
				_view.SetImage(image);
			}
		}

		public void SetDataSourceIndex(int dataSourceIndex)
		{
			_dataSourceIndex = dataSourceIndex;
		}

		protected override void OnAllDataChanged()
		{
			OnDataItemChanged(_dataSourceIndex, 0);
		}

		protected override void OnDataItemChanged(int index1, int index2)
		{
			if (index1 == _dataSourceIndex)
			{
				RefreshFromData();
			}
		}

		public override void SetView(IGenericComponentView view)
		{
			_view = (view as GenericImageComponentView);
		}
	}
}
