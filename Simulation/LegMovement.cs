using UnityEngine;

namespace Simulation
{
	internal sealed class LegMovement
	{
		private LegData _legData;

		private LegMovementData _lightLegMovement;

		private LegMovementData _heavyLegMovement;

		private float _lightLegMass;

		private float _heavyLegMass;

		public float lightLegMass
		{
			set
			{
				_lightLegMass = value;
			}
		}

		public float heavyLegMass => _heavyLegMass;

		public float heightMultiplier
		{
			get;
			set;
		}

		public float idealHeight => Ratio(_lightLegMovement.idealHeight, _heavyLegMovement.idealHeight);

		public float idealCrouchingHeight => Ratio(_lightLegMovement.idealCrouchingHeight, _heavyLegMovement.idealCrouchingHeight);

		public float idealHeightRange => Ratio(_lightLegMovement.idealHeightRange, _heavyLegMovement.idealHeightRange);

		public float jumpHeight => Ratio(_lightLegMovement.jumpHeight, _heavyLegMovement.jumpHeight) * heightMultiplier;

		public float maxUpwardsForce => _legData.massPerLeg * Ratio(_lightLegMovement.maxUpwardsForce, _heavyLegMovement.maxUpwardsForce);

		public float maxLateralForce => _legData.massPerLeg * Ratio(_lightLegMovement.maxLateralForce, _heavyLegMovement.maxLateralForce);

		public float maxTurningForce => _legData.massPerLeg * Ratio(_lightLegMovement.maxTurningForce, _heavyLegMovement.maxTurningForce);

		public float maxDampingForce => _legData.massPerLeg * Ratio(_lightLegMovement.maxDampingForce, _heavyLegMovement.maxDampingForce);

		public float maxStoppedForce => _legData.massPerLeg * Ratio(_lightLegMovement.maxStoppedForce, _heavyLegMovement.maxStoppedForce);

		public float maxNewStoppedForce => _legData.massPerLeg * Ratio(_lightLegMovement.maxNewStoppedForce, _heavyLegMovement.maxNewStoppedForce);

		public float upwardsDampingForce => _legData.massPerLeg * Ratio(_lightLegMovement.upwardsDampingForce, _heavyLegMovement.upwardsDampingForce);

		public float lateralDampForce => _legData.massPerLeg * Ratio(_lightLegMovement.lateralDampForce, _heavyLegMovement.lateralDampForce);

		public float swaggerForce => _legData.massPerLeg * Ratio(_lightLegMovement.swaggerForce, _heavyLegMovement.swaggerForce);

		public LegMovement(CubeLeg leg)
		{
			_legData = leg.legData;
			_lightLegMovement = leg.lightLegMovement;
			_heavyLegMovement = leg.heavyLegMovement;
			_lightLegMass = leg.lightLegMass;
			_heavyLegMass = leg.heavyLegMass;
			heightMultiplier = 1f;
		}

		private float Ratio(float lightValue, float heavyValue)
		{
			float num = Mathf.InverseLerp(_lightLegMass, _heavyLegMass, _legData.massPerLeg);
			return (1f - num) * lightValue + num * heavyValue;
		}
	}
}
