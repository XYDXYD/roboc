using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class SpawnShieldClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private ShieldModuleEventDependency _dependency;

		private BuildShieldParametersData _parametersData = new BuildShieldParametersData();

		[Inject]
		internal DiscShieldFactory discShieldFactory
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as ShieldModuleEventDependency);
			return this;
		}

		public void Execute()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			bool flag = playerTeamsContainer.IsOnMyTeam(TargetType.Player, _dependency.shooterId);
			Vector3 shieldPosition = _dependency.shieldPosition;
			Quaternion shieldRotation = _dependency.shieldRotation;
			_parametersData.SetValues(Vector3.get_zero(), Vector3.get_zero(), null, shieldPosition, shieldRotation, _dependency.shooterId, isMine_: false, flag);
			ShieldEntity shieldEntity = (!flag) ? discShieldFactory.Build("T5_Disc_Shield_Module_Shield_E", _parametersData, hitSomething: false) : discShieldFactory.Build("T5_Disc_Shield_Module_Shield", _parametersData, hitSomething: false);
			shieldEntity.get_gameObject().SetActive(true);
		}
	}
}
