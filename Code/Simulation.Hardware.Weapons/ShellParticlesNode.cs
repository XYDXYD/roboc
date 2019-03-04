using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class ShellParticlesNode : EntityView
	{
		public IWeaponShellParticlesComponent particlesComponent;

		public IHardwareOwnerComponent weaponOwner;

		public IShootingComponent shootingComponent;

		public ShellParticlesNode()
			: this()
		{
		}
	}
}
