namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal sealed class RocketLauncherShootingNode : WeaponShootingNode
	{
		public IHomingProjectileStatsComponent homingProjectileStats;

		public ISplashDamageComponent splashDamageStats;
	}
}
