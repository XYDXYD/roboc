using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System.Collections;

namespace Simulation
{
	internal class SetNonWeaponHealedCubesClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private HealedCubesDependency _dependency;

		[Inject]
		internal HealingManager healingManager
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as HealedCubesDependency);
			return this;
		}

		public void Execute()
		{
			if (_dependency.healedCubes.Count > 0)
			{
				HealCubesImmediate();
			}
		}

		private void HealCubesImmediate()
		{
			PerformHeal(_dependency);
		}

		private IEnumerator HealCubesDelayed(HealedCubesDependency dependency)
		{
			PerformHeal(dependency);
			yield return null;
		}

		private void PerformHeal(HealedCubesDependency dependency)
		{
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(dependency.targetType, dependency.healedMachine);
			healingManager.PerformHealing(dependency.healedCubes, playerFromMachineId, dependency.healedMachine, dependency.typePerformingHealing, dependency.targetType, playEffects: true);
		}
	}
}
