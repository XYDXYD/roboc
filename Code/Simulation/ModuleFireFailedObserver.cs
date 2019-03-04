using System;

namespace Simulation
{
	internal sealed class ModuleFireFailedObserver
	{
		public event Action<ItemCategory, int> OnModuleFireFailed = delegate
		{
		};

		public void ModuleFireFailed(ItemCategory type, int index)
		{
			this.OnModuleFireFailed(type, index);
		}
	}
}
