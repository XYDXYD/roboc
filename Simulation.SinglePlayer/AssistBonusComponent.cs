using System.Collections.Generic;

namespace Simulation.SinglePlayer
{
	internal class AssistBonusComponent : IAssistBonusComponent
	{
		private Dictionary<int, int> _cachedData = new Dictionary<int, int>();

		Dictionary<int, int> IAssistBonusComponent.cachedData
		{
			get
			{
				return _cachedData;
			}
		}

		int IAssistBonusComponent.myTotalHealth
		{
			get;
			set;
		}
	}
}
