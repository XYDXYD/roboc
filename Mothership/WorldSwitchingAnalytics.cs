using System.Collections.Generic;

namespace Mothership
{
	internal class WorldSwitchingAnalytics
	{
		private string _currentGameModeType = string.Empty;

		private string _currentLevelName = string.Empty;

		protected string currentGameModeType => _currentGameModeType;

		protected string currentLevelName => _currentLevelName;

		protected virtual string GetGameModeType()
		{
			return WorldSwitching.GetGameModeType().ToString();
		}

		protected int GetCubeCategoryCount(IEnumerator<InstantiatedCube> cubesEnumerator, CubeCategory category)
		{
			int num = 0;
			while (cubesEnumerator.MoveNext())
			{
				PersistentCubeData persistentCubeData = cubesEnumerator.Current.persistentCubeData;
				if (persistentCubeData.category == category)
				{
					num++;
				}
			}
			return num;
		}

		protected void SetCurrentWorld(string levelName, string gameMode)
		{
			_currentGameModeType = gameMode;
			_currentLevelName = levelName;
		}
	}
}
