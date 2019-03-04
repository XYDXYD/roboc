using Svelto.ECS;

namespace Simulation
{
	internal class AliveStateImplementor : IAliveStateComponent
	{
		private DispatchOnChange<bool> _isAlive;

		public DispatchOnChange<bool> isAlive => _isAlive;

		public AliveStateImplementor(int machineId)
		{
			_isAlive = new DispatchOnChange<bool>(machineId);
		}
	}
}
