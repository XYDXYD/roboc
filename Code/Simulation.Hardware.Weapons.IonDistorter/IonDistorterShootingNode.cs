namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal sealed class IonDistorterShootingNode : WeaponShootingNode
	{
		public IWeaponAnimationComponent animationComponent;

		public IWeaponAccuracyStatsComponent accuracyStats;

		public IIonDistorterProjectileSettingsComponent projectileSettingsComponent;
	}
}
