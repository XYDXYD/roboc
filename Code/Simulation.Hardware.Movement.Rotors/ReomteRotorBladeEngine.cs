using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class ReomteRotorBladeEngine : ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		public const float STOPPED = 0.01f;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<RemoteMachineRotorNode> val = entityViewsDB.QueryEntityViews<RemoteMachineRotorNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				RemoteMachineRotorNode remoteMachineRotorNode = val.get_Item(i);
				if (!remoteMachineRotorNode.machineStunComponent.stunned)
				{
					int num = default(int);
					RotorBladeAudioNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<RotorBladeAudioNode>(remoteMachineRotorNode.get_ID(), ref num);
					if (num > 0)
					{
						ReadInput(remoteMachineRotorNode);
						CalculatePowerValue(remoteMachineRotorNode);
					}
				}
			}
		}

		private void ReadInput(RemoteMachineRotorNode machineNode)
		{
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			IMachineControl machineInput = machineNode.inputWrapperComponent.machineInput;
			if (machineInput.moveUpwards != machineInput.moveDown)
			{
				inputComponent.inputRise = machineInput.moveUpwards;
				inputComponent.inputLower = machineInput.moveDown;
			}
			else
			{
				IRotorInputComponent rotorInputComponent = inputComponent;
				bool inputRise = inputComponent.inputLower = false;
				rotorInputComponent.inputRise = inputRise;
			}
			inputComponent.inputRight = (machineInput.horizontalAxis > 0.01f);
			inputComponent.inputLeft = (machineInput.horizontalAxis < -0.01f);
			inputComponent.inputForward = (machineInput.forwardAxis > 0.01f);
			inputComponent.inputBack = (machineInput.forwardAxis < -0.01f);
		}

		private void CalculatePowerValue(RemoteMachineRotorNode machineNode)
		{
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			IRotorPowerValueComponent powerValueComponent = machineNode.powerValueComponent;
			IAudioLiftingLoweringComponent audioLiftingLoweringComponent = machineNode.audioLiftingLoweringComponent;
			float num = 0f;
			if (inputComponent.inputRise != inputComponent.inputLower)
			{
				num += 1f;
			}
			if (inputComponent.inputRight != inputComponent.inputLeft)
			{
				num += 1f;
			}
			if (inputComponent.inputForward != inputComponent.inputBack)
			{
				num += 1f;
			}
			num = (powerValueComponent.power = num * 0.333333f);
			audioLiftingLoweringComponent.lifting = inputComponent.inputRise;
			audioLiftingLoweringComponent.lowering = inputComponent.inputLower;
		}

		public void Ready()
		{
		}
	}
}
