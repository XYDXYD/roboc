using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Svelto.Ticker.Legacy
{
	public class TickBehaviour : MonoBehaviour
	{
		private List<ILateTickable> _lateTicked = new List<ILateTickable>();

		private List<IPhysicallyTickable> _physicallyTicked = new List<IPhysicallyTickable>();

		private List<ITickable> _ticked = new List<ITickable>();

		private List<string> _namesLate = new List<string>();

		private List<string> _namesPhys = new List<string>();

		private List<string> _names = new List<string>();

		public TickBehaviour()
			: this()
		{
		}

		internal void Add(ITickable tickable)
		{
			_ticked.Add(tickable);
			_names.Add(tickable.ToString());
		}

		internal void AddLate(ILateTickable tickable)
		{
			_lateTicked.Add(tickable);
			_namesLate.Add(tickable.ToString());
		}

		internal void AddPhysic(IPhysicallyTickable tickable)
		{
			_physicallyTicked.Add(tickable);
			_namesPhys.Add(tickable.ToString());
		}

		internal void Remove(ITickable tickable)
		{
			_ticked.Remove(tickable);
			_names.Remove(tickable.ToString());
		}

		internal void RemoveLate(ILateTickable tickable)
		{
			_lateTicked.Remove(tickable);
			_namesLate.Remove(tickable.ToString());
		}

		internal void RemovePhysic(IPhysicallyTickable tickable)
		{
			_physicallyTicked.Remove(tickable);
			_namesPhys.Remove(tickable.ToString());
		}

		private void FixedUpdate()
		{
			for (int num = _physicallyTicked.Count - 1; num >= 0; num--)
			{
				try
				{
					_physicallyTicked[num].PhysicsTick(Time.get_fixedDeltaTime());
				}
				catch (Exception ex)
				{
					Console.LogException(ex);
				}
			}
		}

		private void LateUpdate()
		{
			for (int num = _lateTicked.Count - 1; num >= 0; num--)
			{
				try
				{
					_lateTicked[num].LateTick(Time.get_deltaTime());
				}
				catch (Exception ex)
				{
					Console.LogException(ex);
				}
			}
		}

		private void Update()
		{
			for (int num = _ticked.Count - 1; num >= 0; num--)
			{
				try
				{
					_ticked[num].Tick(Time.get_deltaTime());
				}
				catch (Exception ex)
				{
					Console.LogException(ex);
				}
			}
		}
	}
}
