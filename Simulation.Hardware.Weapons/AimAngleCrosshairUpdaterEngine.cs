using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class AimAngleCrosshairUpdaterEngine : SingleEntityViewEngine<AimAngleCrosshairUpdaterNode>, IQueryingEntityViewEngine, IEngine
	{
		private ITaskRoutine _tick;

		private bool _ticking;

		[Inject]
		private CrosshairController crosshairController
		{
			get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public AimAngleCrosshairUpdaterEngine()
		{
			_tick = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Tick);
		}

		public void Ready()
		{
		}

		private void OnWeaponActiveChanged(int weaponId, bool active)
		{
			if (active && !_ticking)
			{
				_ticking = true;
				_tick.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Add(AimAngleCrosshairUpdaterNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				node.weaponActiveComponent.onActiveChanged.NotifyOnValueSet((Action<int, bool>)OnWeaponActiveChanged);
				if (node.weaponActiveComponent.active && !_ticking)
				{
					_ticking = true;
					_tick.Start((Action<PausableTaskException>)null, (Action)null);
				}
			}
		}

		protected override void Remove(AimAngleCrosshairUpdaterNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				node.weaponActiveComponent.onActiveChanged.StopNotify((Action<int, bool>)OnWeaponActiveChanged);
			}
		}

		private IEnumerator Tick()
		{
			while (_ticking)
			{
				int count = default(int);
				AimAngleCrosshairUpdaterNode[] guns = entityViewsDB.QueryEntityViewsAsArray<AimAngleCrosshairUpdaterNode>(ref count);
				Vector3 dir = default(Vector3);
				int activeCount = 0;
				for (int i = 0; i < count; i++)
				{
					AimAngleCrosshairUpdaterNode aimAngleCrosshairUpdaterNode = guns[i];
					if (aimAngleCrosshairUpdaterNode.weaponActiveComponent.active && aimAngleCrosshairUpdaterNode.ownerComponent.ownedByMe)
					{
						dir += aimAngleCrosshairUpdaterNode.weaponRotationTransformsComponent.verticalTransform.get_forward();
						activeCount++;
					}
				}
				if (activeCount > 0)
				{
					dir /= (float)activeCount;
					dir.Normalize();
					float angleToHorizon = Mathf.Asin(dir.y);
					crosshairController.angleToHorizonDeg = 57.29578f * angleToHorizon;
					yield return null;
				}
				else
				{
					_ticking = false;
				}
			}
		}
	}
}
