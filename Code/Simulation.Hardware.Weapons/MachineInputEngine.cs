using Simulation.Hardware.Cosmetic;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class MachineInputEngine : ITickable, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, ITickableBase, IEngine
	{
		private bool _localPlayerRectifying;

		private readonly FirePressedObservable _firePressedObservable;

		private readonly FireHeldDownObservable _fireHeldDownObservable;

		private readonly TauntPressedObservable _tauntPressedObservable;

		private readonly LocalAlignmentRectifierActivatedObserver _localAlignmentRectifierActivatedObserver;

		[Inject]
		public InputController inputController
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe MachineInputEngine(FirePressedObservable firePressedObservable, FireHeldDownObservable fireHeldDownObservable, LocalAlignmentRectifierActivatedObserver localAlignmentRectifierActivatedObserver, TauntPressedObservable tauntPressedObservable)
		{
			_localAlignmentRectifierActivatedObserver = localAlignmentRectifierActivatedObserver;
			_localAlignmentRectifierActivatedObserver.AddAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_firePressedObservable = firePressedObservable;
			_fireHeldDownObservable = fireHeldDownObservable;
			_tauntPressedObservable = tauntPressedObservable;
		}

		public void Ready()
		{
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_localAlignmentRectifierActivatedObserver.RemoveAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnAlignmentRectifierStarted(ref bool active)
		{
			_localPlayerRectifying = active;
		}

		private static void ProcessInput(MachineInputNode node)
		{
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			IMachineInputComponent machineInput = node.machineInput;
			IMachineControl machineInput2 = machineInput.machineInput;
			int num = (machineInput2.horizontalAxis > 0f) ? 1 : ((machineInput2.horizontalAxis < 0f) ? (-1) : 0);
			int num2 = (machineInput2.forwardAxis > 0f) ? 1 : ((machineInput2.forwardAxis < 0f) ? (-1) : 0);
			int num3 = machineInput2.moveUpwards ? 1 : (machineInput2.moveDown ? (-1) : 0);
			int num4 = machineInput2.strafeLeft ? 1 : (machineInput2.strafeRight ? (-1) : 0);
			machineInput.digitalInput = new Vector4((float)num, (float)num3, (float)num2, (float)num4);
			machineInput.analogInput = new Vector4(machineInput2.horizontalAxis, (float)num3, machineInput2.forwardAxis, (float)num4);
		}

		public void Tick(float deltaSec)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (inputController.Enabled)
			{
				FasterListEnumerator<MachineInputNode> enumerator = entityViewsDB.QueryEntityViews<MachineInputNode>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						MachineInputNode current = enumerator.get_Current();
						if (!_localPlayerRectifying || !current.ownerComponent.ownedByMe)
						{
							ProcessInput(current);
							HandleInput(current.machineInput, current.get_ID());
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
		}

		private void HandleInput(IMachineInputComponent inputComponent, int machineId)
		{
			IMachineControl machineInput = inputComponent.machineInput;
			inputComponent.fire1 = machineInput.fire1;
			if (machineInput.fire1 > 0f)
			{
				inputComponent.firePressed.Dispatch(ref machineId);
				if (inputComponent.fireHeldDown)
				{
					_fireHeldDownObservable.Dispatch(ref machineId);
				}
				else
				{
					_firePressedObservable.Dispatch(ref machineId);
					inputComponent.fireHeldDown = true;
				}
			}
			else
			{
				inputComponent.fireHeldDown = false;
			}
			inputComponent.fire2 = machineInput.fire2;
			inputComponent.zoomPressed.Dispatch(ref machineId);
			if (machineInput.taunt)
			{
				_tauntPressedObservable.Dispatch();
			}
		}
	}
}
