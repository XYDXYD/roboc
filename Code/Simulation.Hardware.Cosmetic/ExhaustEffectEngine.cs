using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Cosmetic
{
	public class ExhaustEffectEngine : IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
			TaskRunner.get_Instance().Run(Tick());
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				yield return null;
				float deltaSec = Time.get_deltaTime();
				FasterReadOnlyList<MachineWithExhaustsEntityView> machines = entityViewsDB.QueryEntityViews<MachineWithExhaustsEntityView>();
				FasterListEnumerator<MachineWithExhaustsEntityView> enumerator = machines.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						MachineWithExhaustsEntityView current = enumerator.get_Current();
						current.audioComponent.aliveExhaustsCount = 0;
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				FasterListEnumerator<ExhaustEffectEntityView> enumerator2 = entityViewsDB.QueryEntityViews<ExhaustEffectEntityView>().GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ExhaustEffectEntityView current2 = enumerator2.get_Current();
						MachineWithExhaustsEntityView machineWithExhaustsEntityView = default(MachineWithExhaustsEntityView);
						if (entityViewsDB.TryQueryEntityView<MachineWithExhaustsEntityView>(current2.ownerComponent.ownerId, ref machineWithExhaustsEntityView))
						{
							if (current2.hardwareDisabledComponent.enabled)
							{
								machineWithExhaustsEntityView.audioComponent.aliveExhaustsCount++;
							}
							bool flag = machineWithExhaustsEntityView.motorForwardsFeedbackComponent.motorForward || machineWithExhaustsEntityView.motorForwardsFeedbackComponent.motorBackward;
							bool flag2 = flag && !current2.hardwareDisabledComponent.disabled;
							if (flag2 != current2.exhaustComponent.idleLoopParticles.get_isEmitting())
							{
								if (flag)
								{
									current2.exhaustComponent.idleLoopParticles.Play();
								}
								else
								{
									current2.exhaustComponent.idleLoopParticles.Stop();
									if (!current2.hardwareDisabledComponent.disabled)
									{
										current2.exhaustComponent.stopParticles.Play();
									}
								}
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2).Dispose();
				}
				FasterListEnumerator<MachineWithExhaustsEntityView> enumerator3 = machines.GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						MachineWithExhaustsEntityView current3 = enumerator3.get_Current();
						GameObject audioGO = current3.audioGoComponent.audioGO;
						IMachineWithExhaustsAudioComponent audioComponent = current3.audioComponent;
						if (audioComponent.aliveExhaustsCount > 0)
						{
							if (audioComponent.aliveExhaustsCountLastFrame != audioComponent.aliveExhaustsCount)
							{
								if (audioComponent.aliveExhaustsCountLastFrame == 0)
								{
									EventManager.get_Instance().PostEvent(audioComponent.exhaustAudioEvent, 0, audioGO);
								}
								EventManager.get_Instance().SetParameter(audioComponent.exhaustAudioEvent, audioComponent.countModulationParameter, (float)audioComponent.aliveExhaustsCount, audioGO);
							}
							if (current3.motorForwardsFeedbackComponent.motorForward || current3.motorForwardsFeedbackComponent.motorBackward)
							{
								if (audioComponent.currentPower < 1f)
								{
									audioComponent.currentPower = Mathf.Min(1f, audioComponent.currentPower + deltaSec / audioComponent.powerRampUpTime);
								}
							}
							else if (audioComponent.currentPower > 0f)
							{
								audioComponent.currentPower = Mathf.Max(0f, audioComponent.currentPower - deltaSec / audioComponent.powerRampUpTime);
							}
							EventManager.get_Instance().SetParameter(audioComponent.exhaustAudioEvent, audioComponent.powerModulationParameter, audioComponent.currentPower, audioGO);
						}
						else if (audioComponent.aliveExhaustsCountLastFrame > 0)
						{
							EventManager.get_Instance().SetParameter(audioComponent.powerModulationParameter, audioComponent.powerModulationParameter, 0f, audioGO);
							EventManager.get_Instance().PostEvent(audioComponent.exhaustAudioEvent, 1, audioGO);
						}
						audioComponent.aliveExhaustsCountLastFrame = audioComponent.aliveExhaustsCount;
					}
				}
				finally
				{
					((IDisposable)enumerator3).Dispose();
				}
			}
		}
	}
}
