using Svelto.ES.Legacy;

namespace Simulation.SinglePlayer
{
	internal class SinglePlayerWeaponFireValidator : IRunOnWorldSwitching, IComponent
	{
		protected bool _acceptDamage = true;

		public bool FadeIn => false;

		public int Priority => 0;

		public float Duration => 0f;

		public void Execute(WorldSwitchMode currentMode)
		{
			_acceptDamage = false;
		}

		public bool ValidateWeaponFire()
		{
			return _acceptDamage;
		}
	}
}
