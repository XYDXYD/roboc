using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.GUI
{
	internal class HealthBarShowEngine : SingleEntityViewEngine<HealthBarShowEntityView>, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		public const int INITIAL_CAPACITY = 10;

		private const string POOL_NAME = "HitHighlights";

		private readonly ITaskRoutine _tick;

		private readonly MachineCpuDataManager _machineCpuDataManager;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public HealthBarShowEngine(MachineCpuDataManager machineCpuDataManager)
		{
			_tick = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Tick);
			_machineCpuDataManager = machineCpuDataManager;
		}

		public void Ready()
		{
			_machineCpuDataManager.OnMachineCpuInitialized += OnCPUInitialised;
			_machineCpuDataManager.OnMachineCpuChanged += OnCPUChanged;
			_tick.Start((Action<PausableTaskException>)null, (Action)null);
		}

		public void OnFrameworkDestroyed()
		{
			_tick.Stop();
			_machineCpuDataManager.OnMachineCpuChanged -= OnCPUChanged;
			_machineCpuDataManager.OnMachineCpuInitialized -= OnCPUInitialised;
		}

		protected override void Add(HealthBarShowEntityView entityView)
		{
			entityView.healthBarComponent.highlightPool.Preallocate("HitHighlights", 10, (Func<HitHighlight>)(() => CreateInstance(entityView.healthBarComponent.hitHighlight)));
			entityView.healthBarComponent.timeSinceLastHit = 0f;
			entityView.healthBarComponent.healthBarGameObject.SetActive(false);
			entityView.healthBarMachineIdComponent.isActive.NotifyOnValueSet((Action<int, bool>)ToggleVisibility);
		}

		protected override void Remove(HealthBarShowEntityView entityView)
		{
			entityView.healthBarMachineIdComponent.isActive.StopNotify((Action<int, bool>)ToggleVisibility);
		}

		private void ToggleVisibility(int entityId, bool show)
		{
			HealthBarShowEntityView healthBarShowEntityView = entityViewsDB.QueryEntityView<HealthBarShowEntityView>(entityId);
			healthBarShowEntityView.healthBarComponent.healthBarGameObject.SetActive(show);
		}

		private void OnCPUInitialised(int machineId, uint cpuValue)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<HealthBarShowEntityView> enumerator = entityViewsDB.QueryEntityViews<HealthBarShowEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					HealthBarShowEntityView current = enumerator.get_Current();
					if (current.healthBarMachineIdComponent.machineId == machineId)
					{
						InitialiseBar(current);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private static void InitialiseBar(HealthBarShowEntityView entityView)
		{
			OnHealthGained(entityView, 1f);
		}

		private void OnCPUChanged(int shooterId, TargetType shooterType, int machineId, float cpuRatio)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<HealthBarShowEntityView> enumerator = entityViewsDB.QueryEntityViews<HealthBarShowEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					HealthBarShowEntityView current = enumerator.get_Current();
					if (current.healthBarMachineIdComponent.machineId == machineId)
					{
						if (current.healthBarComponent.frontBar.get_fillAmount() < cpuRatio)
						{
							OnHealthGained(current, cpuRatio);
						}
						else
						{
							OnHealthLost(current, cpuRatio);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private static void OnHealthGained(HealthBarShowEntityView healthBar, float healthRatio)
		{
			healthBar.healthBarComponent.frontBar.set_fillAmount(healthRatio);
			healthBar.healthBarComponent.backBar.set_fillAmount(healthRatio);
		}

		private static void OnHealthLost(HealthBarShowEntityView healthBar, float healthRatio)
		{
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			float num = healthBar.healthBarComponent.frontBar.get_fillAmount() - healthRatio;
			healthBar.healthBarComponent.frontBar.set_fillAmount(healthRatio);
			if (healthBar.healthBarComponent.timeSinceLastHit > healthBar.healthBarComponent.timeToGroupHits)
			{
				healthBar.healthBarComponent.backBar.set_fillAmount(healthRatio);
			}
			healthBar.healthBarComponent.timeSinceLastHit = 0f;
			HitHighlight hitHighlight = healthBar.healthBarComponent.highlightPool.Use("HitHighlights", (Func<HitHighlight>)(() => CreateInstance(healthBar.healthBarComponent.hitHighlight)));
			int width = healthBar.healthBarComponent.frontBar.get_width();
			hitHighlight.highlightSprite.set_width(Mathf.CeilToInt((float)width * num));
			Vector3 localPosition = hitHighlight.highlightSprite.get_cachedTransform().get_localPosition();
			localPosition.x = (float)width * (healthRatio + num * 0.5f - 0.5f);
			hitHighlight.highlightSprite.get_cachedTransform().set_localPosition(localPosition);
			hitHighlight.highlightAnimation.Play();
			healthBar.healthBarComponent.liveHighlights.Add(hitHighlight);
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				FasterListEnumerator<HealthBarShowEntityView> enumerator = entityViewsDB.QueryEntityViews<HealthBarShowEntityView>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						HealthBarShowEntityView current = enumerator.get_Current();
						UpdateBackBar(current);
						RecycleHighlights(current);
						UpdateGlow(current);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				yield return null;
			}
		}

		private static void UpdateBackBar(HealthBarShowEntityView healthBar)
		{
			if (healthBar.healthBarComponent.timeSinceLastHit < healthBar.healthBarComponent.timeToGroupHits)
			{
				healthBar.healthBarComponent.timeSinceLastHit += Time.get_deltaTime();
				return;
			}
			float num = healthBar.healthBarComponent.backBar.get_fillAmount() - healthBar.healthBarComponent.frontBar.get_fillAmount();
			if (!(num <= 0f))
			{
				float num2 = healthBar.healthBarComponent.backBarSpeed * Time.get_deltaTime();
				if (num2 > num)
				{
					num2 = num;
				}
				UISprite backBar = healthBar.healthBarComponent.backBar;
				backBar.set_fillAmount(backBar.get_fillAmount() - num2);
			}
		}

		private static void RecycleHighlights(HealthBarShowEntityView healthBar)
		{
			for (int num = healthBar.healthBarComponent.liveHighlights.get_Count() - 1; num >= 0; num--)
			{
				HitHighlight hitHighlight = healthBar.healthBarComponent.liveHighlights.get_Item(num);
				if (!hitHighlight.highlightAnimation.get_isPlaying())
				{
					healthBar.healthBarComponent.liveHighlights.RemoveAt(num);
					healthBar.healthBarComponent.highlightPool.Recycle(hitHighlight, "HitHighlights");
				}
			}
		}

		private static HitHighlight CreateInstance(GameObject template)
		{
			return Object.Instantiate<GameObject>(template, template.get_transform().get_parent()).GetComponent<HitHighlight>();
		}

		private void UpdateGlow(HealthBarShowEntityView healthBar)
		{
			if (!healthBar.healthBarMachineIdComponent.isActive.get_value())
			{
				return;
			}
			int machineId = healthBar.healthBarMachineIdComponent.machineId;
			AutoHealEntityView autoHealEntityView = default(AutoHealEntityView);
			if (!entityViewsDB.TryQueryEntityView<AutoHealEntityView>(machineId, ref autoHealEntityView))
			{
				return;
			}
			if (autoHealEntityView.autoHealComponent.timer < healthBar.healthBarComponent.glowStartTime && healthBar.healthBarComponent.frontBar.get_fillAmount() < 0.999f)
			{
				if (!healthBar.healthBarComponent.glowAnimation.get_isPlaying())
				{
					healthBar.healthBarComponent.glowAnimation.Play();
				}
			}
			else if (healthBar.healthBarComponent.glowAnimation.get_isPlaying())
			{
				healthBar.healthBarComponent.glowAnimation.Stop();
			}
		}
	}
}
