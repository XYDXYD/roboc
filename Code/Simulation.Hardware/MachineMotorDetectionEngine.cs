using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware
{
	public class MachineMotorDetectionEngine : IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				FasterListEnumerator<MachineMotorDetectionEntityView> enumerator = entityViewsDB.QueryEntityViews<MachineMotorDetectionEntityView>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						MachineMotorDetectionEntityView current = enumerator.get_Current();
						if (current.functionalsComponent.functionalsEnabled && !current.stunComponent.stunned)
						{
							IInputMotorComponent motorComponent = current.motorComponent;
							Vector4 analogInput = current.inputComponent.analogInput;
							int motorForward;
							if (!(analogInput.z > 0f))
							{
								Vector4 digitalInput = current.inputComponent.digitalInput;
								motorForward = ((digitalInput.z > 0f) ? 1 : 0);
							}
							else
							{
								motorForward = 1;
							}
							motorComponent.motorForward = ((byte)motorForward != 0);
							IInputMotorComponent motorComponent2 = current.motorComponent;
							Vector4 analogInput2 = current.inputComponent.analogInput;
							int motorBackward;
							if (!(analogInput2.z < 0f))
							{
								Vector4 digitalInput2 = current.inputComponent.digitalInput;
								motorBackward = ((digitalInput2.z < 0f) ? 1 : 0);
							}
							else
							{
								motorBackward = 1;
							}
							motorComponent2.motorBackward = ((byte)motorBackward != 0);
						}
						else
						{
							current.motorComponent.motorForward = false;
							current.motorComponent.motorBackward = false;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				yield return null;
			}
		}
	}
}
