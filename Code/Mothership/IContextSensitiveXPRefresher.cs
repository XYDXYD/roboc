using Robocraft.GUI;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;

namespace Mothership
{
	internal interface IContextSensitiveXPRefresher : ITickable, ITickableBase
	{
		void SetDataSources(List<IDataSource> dataSourcesDependantonXP);

		void ClanScreenShown();

		void ClanScreenHidden();
	}
}
