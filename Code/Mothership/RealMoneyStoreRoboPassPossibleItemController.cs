using Services.Web.Photon;
using Svelto.Context;
using Svelto.IoC;

namespace Mothership
{
	internal class RealMoneyStoreRoboPassPossibleItemController : IRealMoneyStoreRoboPassPossibleItemController, IInitialize, IWaitForFrameworkDestruction
	{
		private int _slotId;

		private RealMoneyStoreRoboPassPossibleItemView _view;

		private IRealMoneyStorePossibleRoboPassItemsDataSource _dataSource;

		void IInitialize.OnDependenciesInjected()
		{
		}

		public void RegisterView(RealMoneyStoreRoboPassPossibleItemView possibleItemsView, int slotId)
		{
			_view = possibleItemsView;
			_slotId = slotId;
		}

		public void SetDataSource(IRealMoneyStorePossibleRoboPassItemsDataSource dataSource)
		{
			_dataSource = dataSource;
			_dataSource.OnDataChanged += HandleDataChanged;
		}

		public void OnFrameworkDestroyed()
		{
			if (_dataSource != null)
			{
				_dataSource.OnDataChanged -= HandleDataChanged;
			}
		}

		private void HandleDataChanged(bool dataHasChanged)
		{
			if (_slotId >= _dataSource.GetDataItemsCount())
			{
				_view.get_gameObject().SetActive(false);
				return;
			}
			RoboPassPreviewItemDisplayData dataItem = _dataSource.GetDataItem(_slotId);
			_view.SetItemLabels(dataItem.Name, dataItem.Category);
			_view.SetSprite(dataItem.SpriteName);
			_view.SetSpriteFullSize(dataItem.SpriteFullSize);
			_view.get_gameObject().SetActive(true);
		}
	}
}
