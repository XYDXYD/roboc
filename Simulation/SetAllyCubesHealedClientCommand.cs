using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal class SetAllyCubesHealedClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private ItemDescriptor _itemDescriptor = new WeaponDescriptor(ItemCategory.Nano, ItemSize.T4);

		private HealedAllyCubesDependency _dependency;

		[Inject]
		internal HealingManager healingManager
		{
			private get;
			set;
		}

		[Inject]
		public WeaponFireStateSync fireStateSync
		{
			private get;
			set;
		}

		[Inject]
		public RemoteClientWeaponFire weaponFire
		{
			private get;
			set;
		}

		[Inject]
		public HealingAppliedObservable healingAppliedObservable
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as HealedAllyCubesDependency);
			return this;
		}

		public void Execute()
		{
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			if (_dependency.healedCubes.Count > 0)
			{
				fireStateSync.RecievedHealResponse(_dependency, TargetType.Player);
				_itemDescriptor.itemSize = _dependency.itemSize;
				bool targetIsMe = healingManager.PerformHealing(_dependency.healedCubes, _dependency.shootingPlayerId, _dependency.healedMachine, _dependency.typePerformingHealing, TargetType.Player, playEffects: true);
				RemoteClientWeaponFire weaponFire = this.weaponFire;
				HitCubeInfo hitCubeInfo = _dependency.healedCubes[0];
				weaponFire.FireRemoteClientWeapons(TargetType.Player, hitCubeInfo.gridLoc, targetIsMe, _itemDescriptor, _dependency.shootingMachineId, _dependency.healedMachine, _dependency.hitEffectOffset, _dependency.hitEffectNormal);
				HealingAppliedData healingAppliedData = new HealingAppliedData(_dependency.shootingMachineId, _dependency.healedMachine, _itemDescriptor.GenerateKey());
				healingAppliedObservable.Dispatch(ref healingAppliedData);
			}
		}
	}
}
