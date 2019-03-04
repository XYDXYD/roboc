using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class SpectatorModeCameraSwitcher : MonoBehaviour, IInitialize
	{
		public List<MonoBehaviour> _switchOff;

		public List<MonoBehaviour> _switchOn;

		[Inject]
		internal ISpectatorModeActivator spectatorModeActivator
		{
			private get;
			set;
		}

		public SpectatorModeCameraSwitcher()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			spectatorModeActivator.Register(SwitchCameras);
		}

		private void SwitchCameras(int myKiller, bool enabled)
		{
			for (int i = 0; i < _switchOn.Count; i++)
			{
				_switchOn[i].set_enabled(enabled);
				if (enabled && _switchOn[i] is SimulationTeamCamera)
				{
					(_switchOn[i] as SimulationTeamCamera).SetKiller(myKiller);
				}
			}
			for (int j = 0; j < _switchOff.Count; j++)
			{
				_switchOff[j].set_enabled(!enabled);
			}
		}
	}
}
