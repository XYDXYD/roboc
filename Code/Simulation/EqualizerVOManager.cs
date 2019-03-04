using Simulation.BattleArena.Equalizer;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;
using Utility;

namespace Simulation
{
	internal class EqualizerVOManager : IWaitForFrameworkDestruction
	{
		private EqualizerNotificationObserver _eqObserver;

		[Inject]
		internal VOManager voManager
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		public unsafe EqualizerVOManager(EqualizerNotificationObserver observer)
		{
			_eqObserver = observer;
			_eqObserver.AddAction(new ObserverAction<EqualizerNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_eqObserver.RemoveAction(new ObserverAction<EqualizerNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void OnNotificationReceived(ref EqualizerNotificationDependency parameter)
		{
			switch (parameter.EqualizerNotific)
			{
			case EqualizerNotification.ActivationWarning:
				PlayVO(AudioFabricGameEvents.VO_RBA_EQ_Warning);
				break;
			case EqualizerNotification.Activate:
				if (playerTeamsContainer.IsMyTeam(parameter.TeamID))
				{
					PlayVO(AudioFabricGameEvents.VO_RBA_EQ_DEF_Activate);
				}
				else
				{
					PlayVO(AudioFabricGameEvents.VO_RBA_EQ_ATT_Activate);
				}
				break;
			case EqualizerNotification.Deactivated:
			case EqualizerNotification.Cancelled:
				PlayVO(AudioFabricGameEvents.VO_RBA_EQ_Deactivated);
				break;
			case EqualizerNotification.Defended:
				if (playerTeamsContainer.IsMyTeam(parameter.TeamID))
				{
					PlayVO(AudioFabricGameEvents.VO_RBA_EQ_Defended);
				}
				else
				{
					PlayVO(AudioFabricGameEvents.VO_RBA_EQ_Failed);
				}
				break;
			case EqualizerNotification.Destroyed:
				PlayVO(AudioFabricGameEvents.VO_RBA_EQ_Destroyed);
				break;
			}
		}

		private void PlayVO(AudioFabricGameEvents fabricEvent)
		{
			Console.Log("PlayVO: " + fabricEvent.ToString());
			voManager.PlayVO(AudioFabricEvent.Name(fabricEvent));
		}
	}
}
