using Simulation.Hardware.Weapons;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.GUI
{
	internal sealed class KillNotificationController
	{
		private KillNotificationView _view;

		private readonly PlayerTeamsContainer _playerTeamsContainer;

		private readonly PlayerNamesContainer _playerNamesContainer;

		private readonly Queue<KillNotificationData> _queue = new Queue<KillNotificationData>();

		private WaitForSeconds _waitForKillAnimation;

		private WaitForSeconds _waitForAssistAnimation;

		private WaitForSeconds _waitForDowntime;

		public KillNotificationController(PlayerTeamsContainer playerTeamsContainer, PlayerNamesContainer playerNamesContainer)
		{
			_playerTeamsContainer = playerTeamsContainer;
			_playerNamesContainer = playerNamesContainer;
		}

		public void SetView(KillNotificationView view)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			_view = view;
			_waitForKillAnimation = new WaitForSeconds(_view.animationComponent.get_Item("Stream_Kill").get_length());
			_waitForAssistAnimation = new WaitForSeconds(_view.animationComponent.get_Item("Stream_Assist").get_length());
			_waitForDowntime = new WaitForSeconds(_view.downtime);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleQueue);
		}

		public void OnPlayerKilled(int shooterPlayerId, int killedPlayerId)
		{
			if (_playerTeamsContainer.IsMe(TargetType.Player, shooterPlayerId))
			{
				KillNotificationData killNotificationData = default(KillNotificationData);
				killNotificationData.isAssist = false;
				killNotificationData.victimName = _playerNamesContainer.GetDisplayName(killedPlayerId);
				KillNotificationData item = killNotificationData;
				_queue.Enqueue(item);
			}
		}

		public void OnKillAssistRewarded(int shooterPlayerId, int killedPlayerId)
		{
			KillNotificationData killNotificationData = default(KillNotificationData);
			killNotificationData.isAssist = true;
			killNotificationData.victimName = _playerNamesContainer.GetDisplayName(killedPlayerId);
			KillNotificationData item = killNotificationData;
			_queue.Enqueue(item);
		}

		private IEnumerator HandleQueue()
		{
			while (true)
			{
				if (_queue.Count > 0)
				{
					yield return ShowNotification();
				}
				else
				{
					yield return null;
				}
			}
		}

		private IEnumerator ShowNotification()
		{
			KillNotificationData kill = _queue.Dequeue();
			if (kill.isAssist)
			{
				_view.assistName = kill.victimName;
				_view.assistGOActive = true;
				_view.animationComponent.Play("Stream_Assist");
				yield return _waitForAssistAnimation;
				_view.assistGOActive = false;
			}
			else
			{
				_view.killName = kill.victimName;
				_view.killGOActive = true;
				_view.animationComponent.Play("Stream_Kill");
				yield return _waitForKillAnimation;
				_view.killGOActive = false;
			}
			yield return _waitForDowntime;
		}
	}
}
