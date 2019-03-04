using Simulation.BattleArena;
using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal class DegenerateHealthMachineEntityView : EntityView
	{
		public IAliveStateComponent aliveStateComponent;

		public IMachineOwnerComponent machineOwnerComponent;

		public IOwnerTeamComponent ownerTeamComponent;

		public IInsideFusionShieldComponent insideFusionShieldComponent;

		public IFusionShieldHealthChangeComponent fusionShieldHealthChangeComponent;

		public DegenerateHealthMachineEntityView()
			: this()
		{
		}
	}
}
