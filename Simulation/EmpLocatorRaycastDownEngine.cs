using Svelto.ECS;
using UnityEngine;

namespace Simulation
{
	internal sealed class EmpLocatorRaycastDownEngine : SingleEntityViewEngine<EmpLocatorCountdownManagementNode>, IQueryingEntityViewEngine, IEngine
	{
		private GlowFloorEffectData _data = new GlowFloorEffectData(0, Vector3.get_zero());

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(EmpLocatorCountdownManagementNode node)
		{
			node.activationComponent.activateEmpStun.subscribers += HandleActivateEmpStun;
		}

		protected override void Remove(EmpLocatorCountdownManagementNode node)
		{
			node.activationComponent.activateEmpStun.subscribers -= HandleActivateEmpStun;
		}

		private void HandleActivateEmpStun(IEmpStunActivationComponent sender, int locatorId)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			EmpLocatorEffectsNode empLocatorEffectsNode = default(EmpLocatorEffectsNode);
			if (entityViewsDB.TryQueryEntityView<EmpLocatorEffectsNode>(locatorId, ref empLocatorEffectsNode))
			{
				Vector3 val = empLocatorEffectsNode.transformComponent.empLocatorTransform.get_position();
				RaycastHit val2 = default(RaycastHit);
				if (Physics.Raycast(val, Vector3.get_down(), ref val2, 2000f, 1 << GameLayers.TERRAIN))
				{
					val = val2.get_point();
				}
				_data.SetValues(locatorId, val);
				empLocatorEffectsNode.effectsComponent.playGlowFloorEffect.Dispatch(ref _data);
			}
		}
	}
}
