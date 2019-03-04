using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal class ChaingunSpinEffectEngine : IQueryingEntityViewEngine, ITickable, IEngine, ITickableBase
	{
		private const string SHADER_PARAM_COLOR = "_TintColor";

		private const string SHADER_PARAM_HEAT = "_HeatStrength";

		private MaterialPropertyBlock _propertyBlock;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			_propertyBlock = new MaterialPropertyBlock();
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<ChaingunSpinEffectNode> val = entityViewsDB.QueryEntityViews<ChaingunSpinEffectNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				ChaingunSpinEffectNode node = val.get_Item(i);
				float spinPower = GetSpinPower(node);
				TickVortexMaterial(node, spinPower);
				TickHeatMaterial(node, spinPower, deltaSec);
				TickRotation(node, deltaSec);
			}
		}

		private void TickVortexMaterial(ChaingunSpinEffectNode node, float power)
		{
			float spinVortexThreshold = node.vortexEffectComponent.spinVortexThreshold;
			node.vortexEffectComponent.spinVortexRenderer.get_gameObject().SetActive(power > spinVortexThreshold);
		}

		private float GetVortexAlpha(float power, float threshold)
		{
			if (power >= threshold)
			{
				if (threshold != 1f)
				{
					return (power - threshold) / (1f - threshold);
				}
				return 1f;
			}
			return 0f;
		}

		private void TickHeatMaterial(ChaingunSpinEffectNode node, float power, float deltaSec)
		{
			IWeaponHeatEffectComponent heatEffectComponent = node.heatEffectComponent;
			float heatIncreaseSpeed = heatEffectComponent.heatIncreaseSpeed;
			float heatDecreaseSpeed = heatEffectComponent.heatDecreaseSpeed;
			float currentHeat = heatEffectComponent.currentHeat;
			currentHeat = (heatEffectComponent.currentHeat = Mathf.Clamp01(currentHeat + (power * heatIncreaseSpeed - heatDecreaseSpeed) * deltaSec));
			Renderer[] heatRenderers = heatEffectComponent.heatRenderers;
			foreach (Renderer val in heatRenderers)
			{
				val.GetPropertyBlock(_propertyBlock);
				_propertyBlock.SetFloat("_HeatStrength", currentHeat);
				val.SetPropertyBlock(_propertyBlock);
			}
		}

		private void TickRotation(ChaingunSpinEffectNode node, float deltaSec)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			float angularSpeed = GetAngularSpeed(node);
			if (angularSpeed > 0f)
			{
				Transform spinBarrelTransform = node.transformComponent.spinBarrelTransform;
				spinBarrelTransform.Rotate(new Vector3(0f, 0f, angularSpeed * deltaSec), 1);
			}
		}

		private float GetAngularSpeed(ChaingunSpinEffectNode node)
		{
			float spinPower = GetSpinPower(node);
			if (spinPower <= 0f)
			{
				return 0f;
			}
			float spinBarrelSpeedScale = node.transformComponent.spinBarrelSpeedScale;
			return spinBarrelSpeedScale * spinPower;
		}

		private float GetSpinPower(ChaingunSpinEffectNode node)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			SharedSpinDataNode sharedSpinDataNode = entityViewsDB.QueryIndexableEntityViews<SharedSpinDataNode>().get_Item(node.get_ID());
			return sharedSpinDataNode.sharedChaingunSpinComponent.sharedSpinData.spinPower;
		}
	}
}
