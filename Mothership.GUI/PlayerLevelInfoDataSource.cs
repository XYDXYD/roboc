using Authentication;
using Robocraft.GUI;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using Utility;

namespace Mothership.GUI
{
	internal sealed class PlayerLevelInfoDataSource : DataSourceBase
	{
		public enum PlayerLevelInfoFields
		{
			PLAYER_LEVEL,
			PLAYER_LEVEL_PROGRESS,
			PLAYER_USERNAME,
			PLAYER_DISPLAY_NAME
		}

		private int _playerLevel;

		private float _playerLevelProgress;

		private IServiceRequestFactory _serviceRequestFactory;

		private int _totalPlayerXp;

		public PlayerLevelInfoDataSource(IServiceRequestFactory serviceRequestFactory)
		{
			_serviceRequestFactory = serviceRequestFactory;
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				return 1;
			}
			return 0;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (typeof(T) == typeof(string) && uniqueIdentifier1 == 0)
			{
				string value = GetPlayerLevel().ToString("n0");
				return (T)Convert.ChangeType(value, typeof(T));
			}
			if (typeof(T) == typeof(string) && uniqueIdentifier1 == 2)
			{
				string username = User.Username;
				return (T)Convert.ChangeType(username, typeof(T));
			}
			if (typeof(T) == typeof(string) && uniqueIdentifier1 == 3)
			{
				string displayName = User.DisplayName;
				return (T)Convert.ChangeType(displayName, typeof(T));
			}
			if (typeof(T) == typeof(float) && uniqueIdentifier1 == 1)
			{
				return (T)Convert.ChangeType(_playerLevelProgress, typeof(T));
			}
			Console.LogError("unsupported conversion: Cannot convert data field " + uniqueIdentifier1 + " to " + typeof(T));
			return default(T);
		}

		private int GetPlayerLevel()
		{
			return _playerLevel;
		}

		private float GetPlayerLevelProgress()
		{
			return _playerLevelProgress;
		}

		public override IEnumerator RefreshData()
		{
			yield return PlayerLevelHelper.LoadCurrentPlayerLevel(_serviceRequestFactory, delegate(PlayerLevelAndProgress data)
			{
				_playerLevel = (int)data.playerLevel;
				_playerLevelProgress = data.progressToNextLevel;
				TriggerAllDataChanged();
			}, delegate
			{
				Console.Log("Player level info data source failed to refresh player level");
			});
			yield return null;
		}

		public override void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
		}
	}
}
