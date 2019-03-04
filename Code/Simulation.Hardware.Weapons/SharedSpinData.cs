namespace Simulation.Hardware.Weapons
{
	internal class SharedSpinData
	{
		public float spinPower;

		public bool spinningUp;

		public float fireCooldown = float.MaxValue;

		public float maxFireCooldown;

		public int enabledWeaponCount;
	}
}
