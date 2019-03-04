using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal class ClearAvatarCacheRequest : IClearAvatarCacheRequest, IServiceRequest, ITask, IAbstractTask
	{
		private List<string> _onlyThesePlayers = new List<string>();

		private List<string> _onlyTheseClans = new List<string>();

		public bool isDone
		{
			get;
			private set;
		}

		public void Execute()
		{
			int num = 0;
			int num2 = 0;
			if (_onlyTheseClans != null && _onlyTheseClans.Count > 0)
			{
				num2 = _onlyTheseClans.Count;
			}
			if (_onlyThesePlayers != null && _onlyThesePlayers.Count > 0)
			{
				num = _onlyThesePlayers.Count;
			}
			if (num == 0 && num2 == 0)
			{
				CacheDTO.AvatarCache.ClearCache();
			}
			else
			{
				if (num > 0)
				{
					foreach (string onlyThesePlayer in _onlyThesePlayers)
					{
						CacheDTO.AvatarCache.ClearPlayerAvatar(onlyThesePlayer);
					}
				}
				if (num2 > 0)
				{
					foreach (string onlyTheseClan in _onlyTheseClans)
					{
						CacheDTO.AvatarCache.ClearClanAvatar(onlyTheseClan);
					}
				}
			}
			isDone = true;
		}

		public void Inject(List<string> clearOnlyThesePlayers, List<string> clearOnlyTheseClans)
		{
			_onlyTheseClans = clearOnlyTheseClans;
			_onlyThesePlayers = clearOnlyThesePlayers;
		}
	}
}
