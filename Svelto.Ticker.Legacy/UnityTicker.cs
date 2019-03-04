using UnityEngine;

namespace Svelto.Ticker.Legacy
{
	internal class UnityTicker : ITicker
	{
		private readonly TickBehaviour _ticker;

		public UnityTicker()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			_ticker = Object.FindObjectOfType<TickBehaviour>();
			if (_ticker == null)
			{
				GameObject val = new GameObject("SveltoTicker");
				_ticker = val.AddComponent<TickBehaviour>();
			}
		}

		public void Add(ITickableBase tickable)
		{
			if (tickable is ITickable)
			{
				_ticker.Add(tickable as ITickable);
			}
			if (tickable is IPhysicallyTickable)
			{
				_ticker.AddPhysic(tickable as IPhysicallyTickable);
			}
			if (tickable is ILateTickable)
			{
				_ticker.AddLate(tickable as ILateTickable);
			}
		}

		public void Remove(ITickableBase tickable)
		{
			if (tickable is ITickable)
			{
				_ticker.Remove(tickable as ITickable);
			}
			if (tickable is IPhysicallyTickable)
			{
				_ticker.RemovePhysic(tickable as IPhysicallyTickable);
			}
			if (tickable is ILateTickable)
			{
				_ticker.RemoveLate(tickable as ILateTickable);
			}
		}
	}
}
