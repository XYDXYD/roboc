using Svelto.ECS;

namespace Simulation.Sight
{
	internal class SpottableImplementor : ISpottableComponent
	{
		private DispatchOnChange<bool> _isSpotted;

		public DispatchOnChange<bool> isSpotted => _isSpotted;

		public float spotLastTimeUpdated
		{
			get;
			set;
		}

		public SpottableImplementor(int machineId)
		{
			_isSpotted = new DispatchOnChange<bool>(machineId);
		}
	}
}
