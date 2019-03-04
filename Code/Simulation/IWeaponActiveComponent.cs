using Svelto.ECS;

namespace Simulation
{
	internal interface IWeaponActiveComponent
	{
		bool active
		{
			get;
			set;
		}

		DispatchOnChange<bool> onActiveChanged
		{
			get;
		}
	}
}
