using System;

namespace Robocraft.GUI
{
	internal interface INotifyDataChanged
	{
		event Action onAllDataChanged;

		event Action<int, int> onDataItemChanged;
	}
}
