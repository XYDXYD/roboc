using Fabric;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class VOManager : ITickable, IInitialize, IWaitForFrameworkDestruction, ITickableBase
	{
		private struct QueuedVO
		{
			private const float MAX_DELAY = 30f;

			public readonly string eventName;

			public readonly float eventRequestTime;

			public bool hasExpired => Time.get_time() - eventRequestTime > 30f;

			public QueuedVO(string eventName)
			{
				this.eventName = eventName;
				eventRequestTime = Time.get_time();
			}
		}

		private string _lastEvent;

		private Queue<QueuedVO> _queue = new Queue<QueuedVO>();

		private bool _playerCanSeeMap;

		[Inject]
		internal WorldSwitching _worldSwitch
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			_worldSwitch.OnWorldJustSwitched += OnWorldJustSwitched;
		}

		public void OnFrameworkDestroyed()
		{
			_worldSwitch.OnWorldJustSwitched -= OnWorldJustSwitched;
		}

		private void OnWorldJustSwitched(WorldSwitchMode obj)
		{
			_playerCanSeeMap = true;
		}

		public void Tick(float deltaSec)
		{
			if (_queue.Count > 0)
			{
				QueuedVO queuedVO = _queue.Peek();
				if (TryPlayVO(queuedVO.eventName) || queuedVO.hasExpired)
				{
					_queue.Dequeue();
				}
			}
		}

		public void PlayVO(string eventName)
		{
			if (!TryPlayVO(eventName))
			{
				_queue.Enqueue(new QueuedVO(eventName));
			}
		}

		private bool TryPlayVO(string eventName)
		{
			if (_lastEvent != null && EventManager.get_Instance().IsEventActive(_lastEvent, null))
			{
				return false;
			}
			if (!_playerCanSeeMap)
			{
				return false;
			}
			_lastEvent = eventName;
			EventManager.get_Instance().PostEvent(eventName, 0);
			return true;
		}

		internal void StopAll()
		{
			_queue.Clear();
			if (_lastEvent != null && EventManager.get_Instance().IsEventActive(_lastEvent, null))
			{
				EventManager.get_Instance().PostEvent(_lastEvent, 1);
			}
		}
	}
}
