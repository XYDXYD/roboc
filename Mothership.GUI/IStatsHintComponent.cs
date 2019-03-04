using System.Collections.Generic;

namespace Mothership.GUI
{
	internal interface IStatsHintComponent
	{
		string title
		{
			set;
		}

		string description
		{
			set;
		}

		IList<ItemStat> statLines
		{
			set;
		}

		IList<ItemStat> cpuAndRRLines
		{
			set;
		}
	}
}
