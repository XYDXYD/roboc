using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class PropellerEffectsEngine : IQueryingEntityViewEngine, ITickable, IEngine, ITickableBase
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<MachinePropellerView> val = entityViewsDB.QueryEntityViews<MachinePropellerView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				MachinePropellerView machinePropellerView = val.get_Item(i);
				bool flag = !machinePropellerView.stunComponent.stunned && machinePropellerView.rectifierComponent.functionalsEnabled;
				int num = default(int);
				PropellerGraphicEffectsNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<PropellerGraphicEffectsNode>(machinePropellerView.get_ID(), ref num);
				for (int j = 0; j < num; j++)
				{
					PropellerGraphicEffectsNode propellerGraphicEffectsNode = array[j];
					if (!propellerGraphicEffectsNode.disabledComponent.disabled && flag)
					{
						SpinBlades(propellerGraphicEffectsNode);
					}
					else
					{
						StopBlades(propellerGraphicEffectsNode);
					}
				}
			}
		}

		private void SpinBlades(PropellerGraphicEffectsNode node)
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			Transform spinningTransform = node.spinningComponent.spinningTransform;
			float spinForce = node.spinningComponent.spinForce;
			float num = 0f;
			bool flag = !node.ownerComponent.ownedByMe;
			if (node.inputReceivedComponent.received != 0f || node.forceAppliedComponent.forceApplied)
			{
				num = Mathf.Sign(Vector3.Dot(node.transformComponent.T.get_up(), node.forceAppliedComponent.forceDirection)) * spinForce;
			}
			else if (flag && node.inputReceivedComponent.received != 0f)
			{
				num = (0f - node.inputReceivedComponent.received) * spinForce;
			}
			float currentSpinningSpeed = node.spinningComponent.currentSpinningSpeed;
			currentSpinningSpeed += num;
			float spinDeceleration = node.spinningComponent.spinDeceleration;
			currentSpinningSpeed = ((currentSpinningSpeed > spinDeceleration) ? (currentSpinningSpeed - spinDeceleration) : ((!(currentSpinningSpeed < 0f - spinDeceleration)) ? 0f : (currentSpinningSpeed + spinDeceleration)));
			float num2 = spinForce * 20f;
			float num3 = num2 * 0.5f;
			currentSpinningSpeed = Mathf.Clamp(currentSpinningSpeed, 0f - num2, num2);
			float num4 = Mathf.Abs(currentSpinningSpeed);
			node.spinningComponent.currentSpinningSpeed = currentSpinningSpeed;
			node.spinningComponent.normalizedSpinningSpeed = num4 / num2;
			node.spinningComponent.blurTransform.get_gameObject().SetActive(num4 >= num3);
			float num5 = Mathf.Clamp01((num4 - num3) / (num3 * 0.5f));
			node.spinningComponent.blurRenderer.get_material().SetFloat("_Opacity", num5);
			node.spinningComponent.blurRenderer.get_material().SetFloat("_RotationSpeed", 0f - Mathf.Sign(currentSpinningSpeed));
			float num6 = Mathf.Clamp(currentSpinningSpeed, 0f - num3, num3);
			spinningTransform.Rotate(new Vector3(0f, 0f, num6 * Time.get_deltaTime()), 1);
		}

		private void StopBlades(PropellerGraphicEffectsNode node)
		{
			Transform spinningTransform = node.spinningComponent.spinningTransform;
			ISpinningBladesComponent spinningComponent = node.spinningComponent;
			float num = 0f;
			node.spinningComponent.normalizedSpinningSpeed = num;
			spinningComponent.currentSpinningSpeed = num;
			node.spinningComponent.blurTransform.get_gameObject().SetActive(false);
			node.spinningComponent.blurRenderer.get_material().SetFloat("_Opacity", 0f);
		}
	}
}
