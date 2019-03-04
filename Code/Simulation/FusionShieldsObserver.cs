using System;

namespace Simulation
{
	internal sealed class FusionShieldsObserver
	{
		private Action<int, bool> OnShieldsChangedState = delegate
		{
		};

		public void TriggerShieldStateChanged(int teamId, bool state)
		{
			OnShieldsChangedState(teamId, state);
		}

		internal void RegisterShieldStateChanged(Action<int, bool> action)
		{
			OnShieldsChangedState = (Action<int, bool>)Delegate.Combine(OnShieldsChangedState, action);
		}

		internal void UnregisterShieldStateChanged(Action<int, bool> action)
		{
			OnShieldsChangedState = (Action<int, bool>)Delegate.Remove(OnShieldsChangedState, action);
		}
	}
}
