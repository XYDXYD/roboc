using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class RobotShakeEngine : MultiEntityViewsEngine<RobotShakeShootingEntityView, RobotShakeDamageEntityView>, IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkDestruction, IEngine
	{
		private bool _enabled = true;

		private Dictionary<int, ShakeMovementData> _shakeData = new Dictionary<int, ShakeMovementData>();

		private Dictionary<int, Vector3> _cubePositions = new Dictionary<int, Vector3>();

		private FasterList<Transform> _cubeTransforms = new FasterList<Transform>();

		private int _shakingCubeCount;

		private float _currentCpuPercent = 1f;

		private RobotShakeDamageEntityView _damageEntityView;

		[Inject]
		internal MachineCpuDataManager cpuManager
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer teamsContainer
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
		internal RigidbodyDataContainer rigidbodyDataContainer
		{
			private get;
			set;
		}

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal MachinePreloader machinePreloader
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer playerNamesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal ICursorMode cursorMode
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			cpuManager.OnMachineCpuChanged += HandleOnMachineCpuChanged;
			cpuManager.OnMachineCpuInitialized += HandleOnMachineCpuInitialized;
			gameEndedObserver.OnGameEnded += HandleOnGameEnded;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)WaitToCacheCubeValues);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			cpuManager.OnMachineCpuChanged -= HandleOnMachineCpuChanged;
			cpuManager.OnMachineCpuInitialized -= HandleOnMachineCpuInitialized;
			gameEndedObserver.OnGameEnded -= HandleOnGameEnded;
		}

		void IQueryingEntityViewEngine.Ready()
		{
			TaskRunner.get_Instance().RunOnSchedule(StandardSchedulers.get_updateScheduler(), Tick());
		}

		protected override void Add(RobotShakeShootingEntityView entityView)
		{
			entityView.robotShakeComponent.applyShake.NotifyOnValueSet((Action<int, int>)HandleApplyRobotShake);
		}

		protected override void Add(RobotShakeDamageEntityView entityView)
		{
			_damageEntityView = entityView;
			InitShakeData();
		}

		protected override void Remove(RobotShakeShootingEntityView entityView)
		{
			entityView.robotShakeComponent.applyShake.StopNotify((Action<int, int>)HandleApplyRobotShake);
		}

		protected override void Remove(RobotShakeDamageEntityView entityView)
		{
		}

		private IEnumerator WaitToCacheCubeValues()
		{
			while (!teamsContainer.OwnIdIsRegistered() || !machinePreloader.IsComplete)
			{
				yield return null;
			}
			CacheCubeValues();
		}

		private void HandleOnGameEnded(bool obj)
		{
			_enabled = false;
		}

		private void InitShakeData()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _damageEntityView.robotShakeDamageComponent.maxSimultaneouslyShake; i++)
			{
				_shakeData.Add(i, new ShakeMovementData(0f, Vector3.get_zero(), 0f, null, 0f, new Random(i), ShakeMovementData.MovementType.Translation, available: true));
			}
		}

		private void HandleOnMachineCpuInitialized(int playerId, uint initialCpu)
		{
			if (teamsContainer.IsMe(TargetType.Player, playerId))
			{
				_currentCpuPercent = 1f;
			}
		}

		private void CacheCubeValues()
		{
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			string playerName = playerNamesContainer.GetPlayerName(teamsContainer.localPlayerId);
			PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(playerName);
			FasterList<Renderer> allRenderers = preloadedMachine.allRenderers;
			for (int i = 0; i < allRenderers.get_Count(); i++)
			{
				Transform val = allRenderers.get_Item(i).get_transform();
				Renderer[] componentsInParent = val.GetComponentsInParent<Renderer>(true);
				if (componentsInParent == null || componentsInParent.Length <= 1)
				{
					RobotShakeImplementor[] componentsInParent2 = val.GetComponentsInParent<RobotShakeImplementor>(true);
					if (componentsInParent2 != null && componentsInParent2.Length > 0 && componentsInParent2[0].transformToMove != null)
					{
						val = componentsInParent2[0].transformToMove;
					}
					_cubeTransforms.Add(val);
					_cubePositions.Add(val.GetInstanceID(), val.get_localPosition());
				}
			}
		}

		private void HandleOnMachineCpuChanged(int shooterId, TargetType shooterType, int hitPlayerId, float currentPercent)
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			if (!teamsContainer.IsMe(TargetType.Player, hitPlayerId))
			{
				return;
			}
			if (currentPercent < _currentCpuPercent)
			{
				float num = _currentCpuPercent - currentPercent;
				int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, shooterId);
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, activeMachine);
				int activeMachine2 = playerMachinesContainer.GetActiveMachine(TargetType.Player, hitPlayerId);
				Rigidbody rigidBodyData2 = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, activeMachine2);
				if (rigidBodyData2 != null && rigidBodyData != null)
				{
					Vector3 direction = rigidBodyData.get_worldCenterOfMass() - rigidBodyData2.get_worldCenterOfMass();
					int validShakeKey = GetValidShakeKey();
					if (validShakeKey < 0)
					{
						return;
					}
					float num2 = num * _damageEntityView.robotShakeDamageComponent.damageMagnitudeMultiplier;
					float magnitude = Mathf.Max(num2, _damageEntityView.robotShakeDamageComponent.minimumMagnitude);
					_shakeData[validShakeKey].UpdateValues(Time.get_time(), direction, magnitude, _damageEntityView.robotShakeDamageComponent.damageCurves, _damageEntityView.robotShakeDamageComponent.damageDuration, ShakeMovementData.MovementType.Translation, available: false);
				}
			}
			_currentCpuPercent = currentPercent;
		}

		private void HandleApplyRobotShake(int sender, int weaponId)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			RobotShakeShootingEntityView robotShakeShootingEntityView = entityViewsDB.QueryEntityView<RobotShakeShootingEntityView>(weaponId);
			IRobotShakeComponent robotShakeComponent = robotShakeShootingEntityView.robotShakeComponent;
			if (robotShakeComponent.robotShakeEnabled)
			{
				StartRobotShake(robotShakeComponent.shootRobotCurves, robotShakeShootingEntityView.weaponRotationTransforms.verticalTransform.get_forward(), robotShakeComponent.robotShakeDuration, robotShakeComponent.robotShakeMagnitude);
			}
		}

		private void StartRobotShake(TranslationCurve shootCurves, Vector3 direction, float duration, float magnitude)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (_enabled)
			{
				int validShakeKey = GetValidShakeKey();
				if (validShakeKey >= 0)
				{
					_shakeData[validShakeKey].UpdateValues(Time.get_time(), direction, magnitude, shootCurves, duration, ShakeMovementData.MovementType.Translation, available: false);
				}
			}
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				if (_damageEntityView != null && cursorMode.currentMode == Mode.Lock)
				{
					for (int i = 0; i < _damageEntityView.robotShakeDamageComponent.maxSimultaneouslyShake; i++)
					{
						if (!_shakeData[i].available)
						{
							HandleShake(i);
						}
					}
				}
				yield return null;
			}
		}

		private void HandleShake(int key)
		{
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			float time = Time.get_time() - _shakeData[key].startingTime;
			float? x = MovementCurve.Evaluate(((TranslationCurve)_shakeData[key].curves).x, time, _shakeData[key].duration, _shakeData[key].random);
			float? y = MovementCurve.Evaluate(((TranslationCurve)_shakeData[key].curves).y, time, _shakeData[key].duration, _shakeData[key].random);
			float? z = MovementCurve.Evaluate(((TranslationCurve)_shakeData[key].curves).z, time, _shakeData[key].duration, _shakeData[key].random);
			int num = 0;
			while (true)
			{
				if (num >= _cubeTransforms.get_Count())
				{
					return;
				}
				if (!x.HasValue && !y.HasValue && !z.HasValue)
				{
					_shakingCubeCount--;
					if (_shakingCubeCount <= 0)
					{
						break;
					}
					_shakeData[key].available = true;
				}
				else
				{
					Vector3 val = CalculateMovementOffset(_cubeTransforms.get_Item(num), x, y, z, _shakeData[key].direction, _shakeData[key].magnitude);
					Vector3 val2 = Vector3.get_zero();
					if (_shakeData[key].previousMovementValues.ContainsKey(_cubeTransforms.get_Item(num).GetInstanceID()))
					{
						val2 = _shakeData[key].previousMovementValues[_cubeTransforms.get_Item(num).GetInstanceID()];
					}
					else
					{
						_shakingCubeCount++;
					}
					Vector3 val3 = val - val2;
					_shakeData[key].previousMovementValues[_cubeTransforms.get_Item(num).GetInstanceID()] = val;
					_cubeTransforms.get_Item(num).Translate(val3, 0);
				}
				num++;
			}
			_shakingCubeCount = 0;
			for (int i = 0; i < _cubeTransforms.get_Count(); i++)
			{
				_cubeTransforms.get_Item(i).set_localPosition(_cubePositions[_cubeTransforms.get_Item(i).GetInstanceID()]);
			}
		}

		private Vector3 CalculateMovementOffset(Transform target, float? x, float? y, float? z, Vector3 direction, float magnitude)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			direction *= -1f;
			direction.Normalize();
			Vector3 val = default(Vector3);
			val.x = ((!x.HasValue) ? 0f : x.Value);
			val.y = ((!y.HasValue) ? 0f : y.Value);
			val.z = ((!z.HasValue) ? 0f : z.Value);
			Quaternion val2 = Quaternion.LookRotation(direction, Vector3.get_up());
			val = val2 * val;
			val *= magnitude;
			return val;
		}

		private int GetValidShakeKey()
		{
			for (int i = 0; i < _damageEntityView.robotShakeDamageComponent.maxSimultaneouslyShake; i++)
			{
				if (_shakeData[i].available)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
