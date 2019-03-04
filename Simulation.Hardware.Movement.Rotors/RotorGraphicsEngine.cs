using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class RotorGraphicsEngine : SingleEntityViewEngine<RotorBladeGraphicsNode>, ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		[Inject]
		internal PlayerStrafeDirectionManager strafeDirectionManager
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		protected override void Add(RotorBladeGraphicsNode entityView)
		{
		}

		protected override void Remove(RotorBladeGraphicsNode entityView)
		{
			RotorGraphics rotorGraphics = entityView.rotorGraphicsComponent.rotorGraphics;
			rotorGraphics.groundEffect.get_gameObject().SetActive(false);
		}

		public void Tick(float deltaTime)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<MachineRotorGraphicsNode> val = entityViewsDB.QueryEntityViews<MachineRotorGraphicsNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				MachineRotorGraphicsNode machineRotorGraphicsNode = val.get_Item(i);
				if (machineRotorGraphicsNode.machineStunComponent.stunned)
				{
					continue;
				}
				IRotorPowerValueComponent powerValueComponent = machineRotorGraphicsNode.powerValueComponent;
				powerValueComponent.currentPower += Mathf.Sign(powerValueComponent.power - powerValueComponent.currentPower) * Mathf.Min(powerValueComponent.powerChangeRate * deltaTime, Mathf.Abs(powerValueComponent.power - powerValueComponent.currentPower));
				int num = default(int);
				RotorBladeGraphicsNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<RotorBladeGraphicsNode>(machineRotorGraphicsNode.get_ID(), ref num);
				for (int j = 0; j < num; j++)
				{
					RotorBladeGraphicsNode rotorBladeGraphicsNode = array[j];
					if (!rotorBladeGraphicsNode.disabledComponent.disabled && !rotorBladeGraphicsNode.visibilityComponent.offScreen)
					{
						RotateBlade(rotorBladeGraphicsNode, powerValueComponent.currentPower);
						TiltBlade(machineRotorGraphicsNode, rotorBladeGraphicsNode, deltaTime);
						PlaceEffects(rotorBladeGraphicsNode);
					}
					else
					{
						HideEffects(rotorBladeGraphicsNode);
					}
				}
			}
		}

		private void RotateBlade(RotorBladeGraphicsNode rotor, float currentPower)
		{
			RotorGraphics rotorGraphics = rotor.rotorGraphicsComponent.rotorGraphics;
			Transform spinningPivot = rotor.spinningPivotTransformComponent.spinningPivot;
			if (spinningPivot != null)
			{
				float num = Mathf.Clamp01(1f - currentPower) * rotorGraphics.rotateRatePerFrame + Mathf.Clamp01(currentPower) * rotorGraphics.rotateRateFastPerFrame;
				num *= rotorGraphics.rotateRandomScale;
				spinningPivot.Rotate(0f, num, 0f);
			}
		}

		private void TiltBlade(MachineRotorGraphicsNode machineNode, RotorBladeGraphicsNode rotor, float deltaTime)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			RotorData rotorData = rotor.rotorDataComponent.rotorData;
			RotorGraphics rotorGraphics = rotor.rotorGraphicsComponent.rotorGraphics;
			Transform tiltPivot = rotor.tiltPivotTransformComponent.tiltPivot;
			if (tiltPivot != null && rotorData.hoveringRotor)
			{
				Vector3 val = Vector3.get_zero();
				IRotorInputComponent inputComponent = machineNode.inputComponent;
				if (inputComponent.inputForward)
				{
					val += Vector3.get_right();
				}
				if (inputComponent.inputBack)
				{
					val += Vector3.get_left();
				}
				val = ((!machineNode.ownerComponent.ownedByMe || !strafeDirectionManager.strafingEnabled) ? (val + CalculateLocalAxisLegacy(machineNode, rotorData)) : (val + CalculateLocalAxisStrafe(machineNode, rotorData)));
				Vector3 val2 = machineNode.rigidbodyComponent.rb.get_transform().get_rotation() * val;
				Quaternion val3 = Quaternion.AngleAxis(rotorGraphics.tiltAmount, val2);
				rotorGraphics.currentTilt = Quaternion.RotateTowards(rotorGraphics.currentTilt, val3, rotorGraphics.tiltRate * deltaTime);
				tiltPivot.set_localRotation(Quaternion.get_identity());
				tiltPivot.Rotate(rotorGraphics.currentTilt.get_eulerAngles(), 0);
			}
		}

		private Vector3 CalculateLocalAxisLegacy(MachineRotorGraphicsNode machineNode, RotorData rotorData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			if (inputComponent.inputRight)
			{
				val = ((!rotorData.xInputFlipped) ? (val + Vector3.get_back()) : (val + Vector3.get_forward()));
			}
			if (inputComponent.inputLeft)
			{
				val = ((!rotorData.xInputFlipped) ? (val + Vector3.get_forward()) : (val + Vector3.get_back()));
			}
			return val;
		}

		private Vector3 CalculateLocalAxisStrafe(MachineRotorGraphicsNode machineNode, RotorData rotorData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			if (inputComponent.inputRight)
			{
				val += Vector3.get_back();
			}
			if (inputComponent.inputLeft)
			{
				val += Vector3.get_forward();
			}
			if (strafeDirectionManager.IsAngleToStraightGreaterThanThreshold())
			{
				if (strafeDirectionManager.angleToStraight > 0f)
				{
					val = ((!rotorData.xInputFlipped) ? (val + Vector3.get_forward()) : (val + Vector3.get_back()));
				}
				else if (strafeDirectionManager.angleToStraight < 0f)
				{
					val = ((!rotorData.xInputFlipped) ? (val + Vector3.get_back()) : (val + Vector3.get_forward()));
				}
			}
			return val;
		}

		private void PlaceEffects(RotorBladeGraphicsNode rotor)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			RotorGraphics rotorGraphics = rotor.rotorGraphicsComponent.rotorGraphics;
			Transform groundEffect = rotorGraphics.groundEffect;
			if (!(groundEffect != null))
			{
				return;
			}
			Vector3 position = rotor.transformComponent.T.get_position();
			Vector3 val = -rotor.transformComponent.T.get_up();
			float effectDistance = rotorGraphics.effectDistance;
			RaycastHit val2 = default(RaycastHit);
			bool flag = Physics.Raycast(position, val, ref val2, effectDistance, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK);
			RaycastHit val3 = default(RaycastHit);
			bool flag2 = Physics.Raycast(position, -val, ref val3, effectDistance, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK);
			RaycastHit val4 = val2;
			if (flag || flag2)
			{
				val4 = ((flag != flag2) ? ((!flag) ? val3 : val2) : ((!(val2.get_distance() <= val3.get_distance())) ? val3 : val2));
				rotorGraphics.groundEffectHidden = false;
				groundEffect.get_gameObject().SetActive(true);
				groundEffect.set_position(val4.get_point());
				groundEffect.LookAt(groundEffect.get_position() - val4.get_normal());
				groundEffect.Rotate(rotorGraphics.effectRotation, 1);
				for (int i = 0; i < rotorGraphics.particleSystems.Length; i++)
				{
					ParticleSystem val5 = rotorGraphics.particleSystems[i];
					if (val5 != null)
					{
						Color startColor = val5.get_startColor();
						startColor.a = Mathf.Clamp01(1f - val4.get_distance() / effectDistance);
						val5.set_startColor(startColor);
					}
				}
			}
			else
			{
				HideEffects(rotor);
			}
		}

		private void HideEffects(RotorBladeGraphicsNode rotor)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			RotorGraphics rotorGraphics = rotor.rotorGraphicsComponent.rotorGraphics;
			if (rotorGraphics.groundEffectHidden)
			{
				return;
			}
			rotorGraphics.groundEffectHidden = true;
			for (int i = 0; i < rotorGraphics.particleSystems.Length; i++)
			{
				ParticleSystem val = rotorGraphics.particleSystems[i];
				if (val != null)
				{
					Color startColor = val.get_startColor();
					startColor.a = 0f;
					val.set_startColor(startColor);
				}
			}
			rotorGraphics.groundEffect.get_gameObject().SetActive(false);
		}

		public void Ready()
		{
		}
	}
}
