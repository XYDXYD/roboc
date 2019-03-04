using Robocraft.GUI;
using Svelto.Context;
using Svelto.Tasks;
using System;

namespace Mothership.GUI
{
	internal sealed class PlayerLevelController : IWaitForFrameworkInitialization
	{
		private IDataSource _playerLevelDataSource;

		private IPlayerLevelView _view;

		private PlayerLevelNeedRefreshObserver _observer;

		public PlayerLevelController(IDataSource dataSource, PlayerLevelNeedRefreshObserver observer)
		{
			_playerLevelDataSource = dataSource;
			_observer = observer;
			_observer.AddAction((Action)delegate
			{
				TaskRunner.get_Instance().Run(_playerLevelDataSource.RefreshData());
			});
		}

		public void RegisterToContextNotifier(IContextNotifer contextNotifier)
		{
			contextNotifier.AddFrameworkInitializationListener(this);
		}

		public void OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().Run(_playerLevelDataSource.RefreshData());
			_view.Show();
		}

		public void SetView(IPlayerLevelView view)
		{
			_view = view;
		}

		public void Hide()
		{
			_view.Hide();
		}

		public void Show()
		{
			_view.Show();
		}
	}
}
