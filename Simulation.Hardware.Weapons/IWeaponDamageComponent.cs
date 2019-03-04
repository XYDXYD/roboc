using Svelto.ECS.Legacy;

namespace Simulation.Hardware.Weapons
{
	internal interface IWeaponDamageComponent
	{
		int numHits
		{
			get;
			set;
		}

		HitResult[] hitResults
		{
			get;
			set;
		}

		int hitDepth
		{
			get;
		}

		Dispatcher<IWeaponDamageComponent, int> weaponDamage
		{
			get;
		}
	}
}
