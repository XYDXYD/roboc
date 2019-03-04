using Svelto.ECS.Legacy;

namespace Simulation.Hardware.Weapons
{
	internal interface IHitSomethingComponent
	{
		Dispatcher<IHitSomethingComponent, HitInfo> hitEnemy
		{
			get;
		}

		Dispatcher<IHitSomethingComponent, HitInfo> hitAlly
		{
			get;
		}

		Dispatcher<IHitSomethingComponent, HitInfo> hitEnemySplash
		{
			get;
		}

		Dispatcher<IHitSomethingComponent, HitInfo> hitSelf
		{
			get;
		}

		Dispatcher<IHitSomethingComponent, HitInfo> hitEnvironment
		{
			get;
		}

		Dispatcher<IHitSomethingComponent, HitInfo> hitProtonium
		{
			get;
		}

		Dispatcher<IHitSomethingComponent, HitInfo> hitFusionShield
		{
			get;
		}

		Dispatcher<IHitSomethingComponent, HitInfo> hitSecondaryImpact
		{
			get;
		}

		Dispatcher<IHitSomethingComponent, HitInfo> hitEqualizer
		{
			get;
		}
	}
}
