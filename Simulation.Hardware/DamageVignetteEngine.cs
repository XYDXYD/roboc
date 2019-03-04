using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware
{
	internal sealed class DamageVignetteEngine : SingleEntityViewEngine<DamageVignetteEntityView>, IInitialize, IWaitForFrameworkDestruction
	{
		private float _currentCpuPercent = 1f;

		private DamageVignetteIndicator _damageIndicatorPrefab;

		private Transform _damageIndicatorContainer;

		private Dictionary<int, DamageVignetteData> _activeDamages = new Dictionary<int, DamageVignetteData>();

		[Inject]
		internal DamageVignetteIndicatorPool damageIndicatorPool
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

		void IInitialize.OnDependenciesInjected()
		{
			cpuManager.OnMachineCpuChanged += HandleOnMachineCpuChanged;
			cpuManager.OnMachineCpuInitialized += HandleOnMachineCpuInitialized;
			TaskRunner.get_Instance().RunOnSchedule(StandardSchedulers.get_updateScheduler(), Tick());
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			cpuManager.OnMachineCpuChanged -= HandleOnMachineCpuChanged;
			cpuManager.OnMachineCpuInitialized -= HandleOnMachineCpuInitialized;
		}

		protected override void Add(DamageVignetteEntityView entityView)
		{
			_damageIndicatorPrefab = entityView.damageVignetteComponent.IndicatorPrefab;
			_damageIndicatorContainer = _damageIndicatorPrefab.get_transform().get_parent();
			damageIndicatorPool.Preallocate(_damageIndicatorPrefab.get_name(), 4, (Func<DamageVignetteIndicator>)OnFirstDamageIndicatorInit);
		}

		protected override void Remove(DamageVignetteEntityView entityView)
		{
		}

		private void HandleOnMachineCpuInitialized(int playerId, uint initialCpu)
		{
			if (teamsContainer.IsMe(TargetType.Player, playerId))
			{
				_currentCpuPercent = 1f;
			}
		}

		private DamageVignetteIndicator OnFirstDamageIndicatorInit()
		{
			return Object.Instantiate<DamageVignetteIndicator>(_damageIndicatorPrefab);
		}

		private void HandleOnMachineCpuChanged(int shooterId, TargetType shooterType, int hitPlayerId, float currentPercent)
		{
			if (!teamsContainer.IsMe(TargetType.Player, hitPlayerId))
			{
			}
		}

		private IEnumerator Tick()
		{
			yield break;
		}

		private void HandleDamage(int key)
		{
			DamageVignetteData damageVignetteData = _activeDamages[key];
			UpdateDamage(damageVignetteData.DamageIndicator, damageVignetteData.ShooterRB, damageVignetteData.PlayerRB);
		}

		private void UpdateDamage(DamageVignetteIndicator damageIndicator, Rigidbody shooterRB, Rigidbody playerRB)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			if (shooterRB != null && playerRB != null)
			{
				Vector3 val = shooterRB.get_worldCenterOfMass() - playerRB.get_worldCenterOfMass();
				Vector3 forward = Camera.get_main().get_transform().get_forward();
				Vector3 right = Camera.get_main().get_transform().get_right();
				forward.y = 0f;
				forward.Normalize();
				right.y = 0f;
				val.y = 0f;
				val.Normalize();
				float num = Mathf.Clamp(Vector3.Dot(forward, val), -1f, 1f);
				float num2 = Mathf.Acos(num) * 57.29578f;
				if (Vector3.Dot(right, val) > 0f)
				{
					num2 *= -1f;
				}
				damageIndicator.Pivot.set_rotation(Quaternion.Euler(0f, 0f, num2));
				UpdateIndicator(damageIndicator, num2);
			}
		}

		private void UpdateIndicator(DamageVignetteIndicator damageIndicator, float pivotAngle)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			Vector3 up = damageIndicator.Pivot.get_up();
			up.Normalize();
			float num = Mathf.Atan2(up.y, up.x);
			float num2 = Mathf.Clamp(Mathf.Cos(num) * (float)Screen.get_width() + (float)Screen.get_width() * 0.5f, 0f, (float)Screen.get_width());
			float num3 = Mathf.Clamp(Mathf.Sin(num) * (float)Screen.get_height() + (float)Screen.get_height() * 0.5f, 0f, (float)Screen.get_height());
			num2 -= (float)Screen.get_width() * 0.5f;
			num3 -= (float)Screen.get_height() * 0.5f;
			damageIndicator.Indicator.set_rotation(Quaternion.Euler(0f, 0f, pivotAngle));
			Transform indicator = damageIndicator.Indicator;
			float num4 = num2;
			float num5 = num3;
			Vector3 localPosition = damageIndicator.Indicator.get_localPosition();
			indicator.set_localPosition(new Vector3(num4, num5, localPosition.z));
		}
	}
}
