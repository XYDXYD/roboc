using Battle;
using Commands.Client;
using Events.Dependencies;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal sealed class IonDistorterCollisionEngine : SingleEntityViewEngine<IonDistorterProjectileNode>, IInitialize
	{
		private Dictionary<int, IonDistorterProjectileNode> _projectiles = new Dictionary<int, IonDistorterProjectileNode>(50);

		private FasterList<HitResult> _hitResultsList = new FasterList<HitResult>(20);

		public FasterList<Vector3> hitPoints = new FasterList<Vector3>(20);

		public FasterList<Vector3> hitNormals = new FasterList<Vector3>(20);

		public FasterList<bool> hitSelfList = new FasterList<bool>(20);

		public FasterList<bool> hitList = new FasterList<bool>(20);

		public FasterList<TargetType> targetTypeList = new FasterList<TargetType>(20);

		private bool _playedSoundForFusionShield;

		private HashSet<int> _soundPlayedMachines = new HashSet<int>();

		private BroadcastMultipleMissesClientCommand _broadcastMissClientCommand;

		private MultipleFireMissesDependency _fireMissDependency = new MultipleFireMissesDependency();

		[Inject]
		internal MachineRootContainer machineRootContainer
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

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			get;
			private set;
		}

		[Inject]
		internal BattleTimer battleTimer
		{
			get;
			private set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_broadcastMissClientCommand = commandFactory.Build<BroadcastMultipleMissesClientCommand>();
		}

		protected override void Add(IonDistorterProjectileNode node)
		{
			_projectiles.Add(node.get_ID(), node);
			node.collisionComponent.checkCollision.subscribers += CheckCollisions;
		}

		protected override void Remove(IonDistorterProjectileNode node)
		{
			_projectiles.Remove(node.get_ID());
			node.collisionComponent.checkCollision.subscribers -= CheckCollisions;
		}

		private void CheckCollisions(IIonDistorterCollisonComponent sender, int weaponId)
		{
			if (_projectiles.TryGetValue(weaponId, out IonDistorterProjectileNode value) && value.entitySourceComponent.isLocal)
			{
				RayCastForCollisions(value);
				_soundPlayedMachines.Clear();
				_playedSoundForFusionShield = false;
			}
		}

		private void RayCastForCollisions(IonDistorterProjectileNode currentProjectile)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			ClearLists();
			int value = currentProjectile.get_ID();
			float maxRange = currentProjectile.projectileRangeComponent.maxRange;
			Vector3 position = currentProjectile.transformComponent.T.get_position();
			WeaponRaycastUtility.Ray ray = default(WeaponRaycastUtility.Ray);
			WeaponRaycastUtility.Parameters parameters = default(WeaponRaycastUtility.Parameters);
			HitResult hitResult = default(HitResult);
			ray.startPosition = position;
			ray.range = maxRange;
			parameters.machineManager = machineManager;
			parameters.machineRootContainer = machineRootContainer;
			parameters.playerMachinesContainer = playerMachinesContainer;
			parameters.playerTeamsContainer = playerTeamsContainer;
			parameters.shooterId = currentProjectile.projectileOwnerComponent.ownerId;
			parameters.isShooterAi = currentProjectile.projectileOwnerComponent.isAi;
			parameters.fusionShieldTag = "EnemyFusionShield";
			FasterList<Vector3> projectileDirections = currentProjectile.collisionComponent.projectileDirections;
			int num = 0;
			for (int i = 0; i < projectileDirections.get_Count(); i++)
			{
				ray.direction = projectileDirections.get_Item(i);
				if (WeaponRaycastUtility.RaycastWeaponHit(ref ray, ref parameters, ref hitResult))
				{
					bool playEnvSound = num++ < 4;
					HandleRaycastHit(currentProjectile, hitResult, playEnvSound);
				}
			}
			if (_hitResultsList.get_Count() > 0)
			{
				IWeaponDamageComponent weaponDamageComponent = currentProjectile.weaponDamageComponent;
				weaponDamageComponent.hitResults = _hitResultsList.ToArray();
				weaponDamageComponent.numHits = _hitResultsList.get_Count();
				weaponDamageComponent.weaponDamage.Dispatch(ref value);
			}
			SendFireEffectCommand(currentProjectile);
		}

		private void HandleRaycastHit(IonDistorterProjectileNode currentProjectile, HitResult hitResult, bool playEnvSound)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			bool flag = true;
			Quaternion rotation = Quaternion.LookRotation(hitResult.normal);
			TargetType targetType = hitResult.targetType;
			if (LayerToTargetType.IsTargetDestructible(targetType))
			{
				bool playSound = true;
				int hitTargetMachineId = hitResult.hitTargetMachineId;
				if (_soundPlayedMachines.Contains(hitTargetMachineId))
				{
					playSound = false;
				}
				else
				{
					_soundPlayedMachines.Add(hitTargetMachineId);
				}
				int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(targetType, hitResult.hitTargetMachineId);
				bool targetIsMe = playerTeamsContainer.IsMe(targetType, playerFromMachineId);
				bool shooterIsMe = playerTeamsContainer.IsMe(TargetType.Player, currentProjectile.projectileOwnerComponent.ownerId);
				if (!hitResult.hitAlly && !hitResult.hitSelf && !hitResult.hitOwnBase)
				{
					flag = false;
					DetermineCubeHit(hitResult.hitPoint, rotation, hitResult.hitSelf, targetType, currentProjectile, targetIsMe, shooterIsMe, playSound);
					if (currentProjectile.entitySourceComponent.isLocal)
					{
						_hitResultsList.Add(hitResult);
					}
				}
				if (flag)
				{
					DeterminePropHit(ref hitResult, rotation, targetType, currentProjectile, playSound);
					UpdateLists(hitResult, targetType);
				}
			}
			else
			{
				if (targetType == TargetType.Environment)
				{
					DeterminePropHit(ref hitResult, rotation, targetType, currentProjectile, playEnvSound);
				}
				else
				{
					DeterminePropHit(ref hitResult, rotation, targetType, currentProjectile, !_playedSoundForFusionShield);
					_playedSoundForFusionShield = true;
				}
				UpdateLists(hitResult, targetType);
			}
		}

		private void UpdateLists(HitResult hitResult, TargetType targetType)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			hitPoints.Add(hitResult.hitPoint);
			hitNormals.Add(hitResult.normal);
			hitList.Add(true);
			hitSelfList.Add(hitResult.hitSelf || hitResult.hitOwnBase || hitResult.hitAlly);
			targetTypeList.Add(targetType);
		}

		private void ClearLists()
		{
			hitPoints.FastClear();
			hitNormals.FastClear();
			hitList.FastClear();
			hitSelfList.FastClear();
			targetTypeList.FastClear();
			_hitResultsList.FastClear();
		}

		private void SendFireEffectCommand(IonDistorterProjectileNode currentProjectile)
		{
			_fireMissDependency.SetVariables(hitPoints.get_Count(), currentProjectile.projectileOwnerComponent.machineId, currentProjectile.itemDescriptorComponent.itemDescriptor, hitPoints, hitNormals, hitSelfList, hitList, battleTimer.SecondsSinceGameInitialised, targetTypeList);
			_broadcastMissClientCommand.Inject(_fireMissDependency).Execute();
		}

		private void DetermineCubeHit(Vector3 hitPosition, Quaternion rotation, bool hitSelf, TargetType targetType, IonDistorterProjectileNode projectile, bool targetIsMe, bool shooterIsMe, bool playSound)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			HitInfo value = new HitInfo(targetType, projectile.itemDescriptorComponent.itemDescriptor, projectile.projectileOwnerComponent.isEnemy, hit_: true, hitSelf, hitPosition, rotation, Vector3.get_up(), targetIsMe, shooterIsMe, targetOnMyTeam_: false, null, 0, playSound);
			IHitSomethingComponent hitSomethingComponent = projectile.hitSomethingComponent;
			switch (targetType)
			{
			case TargetType.Environment:
			case TargetType.FusionShield:
				break;
			case TargetType.Player:
				hitSomethingComponent.hitEnemy.Dispatch(ref value);
				break;
			case TargetType.TeamBase:
				hitSomethingComponent.hitProtonium.Dispatch(ref value);
				break;
			case TargetType.EqualizerCrystal:
				hitSomethingComponent.hitEqualizer.Dispatch(ref value);
				break;
			}
		}

		private void DeterminePropHit(ref HitResult hitResult, Quaternion rotation, TargetType targetType, IonDistorterProjectileNode projectile, bool playSound)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			HitInfo value = new HitInfo(targetType, projectile.itemDescriptorComponent.itemDescriptor, projectile.projectileOwnerComponent.isEnemy, hit_: true, hitSelf_: false, hitResult.hitPoint, rotation, Vector3.get_up(), targetIsMe_: false, shooterIsMe_: false, targetOnMyTeam_: false, null, 0, playSound);
			IHitSomethingComponent hitSomethingComponent = projectile.hitSomethingComponent;
			switch (targetType)
			{
			case TargetType.Player:
				if (hitResult.hitSelf || hitResult.hitAlly || hitResult.hitOwnBase)
				{
					hitSomethingComponent.hitSelf.Dispatch(ref value);
				}
				break;
			case TargetType.TeamBase:
				if (hitResult.hitOwnBase)
				{
					hitSomethingComponent.hitSelf.Dispatch(ref value);
				}
				break;
			case TargetType.EqualizerCrystal:
				if (hitResult.hitOwnBase)
				{
					hitSomethingComponent.hitSelf.Dispatch(ref value);
				}
				break;
			case TargetType.FusionShield:
				hitSomethingComponent.hitFusionShield.Dispatch(ref value);
				break;
			case TargetType.Environment:
				hitSomethingComponent.hitEnvironment.Dispatch(ref value);
				break;
			}
		}
	}
}
