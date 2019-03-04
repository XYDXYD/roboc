using Simulation.Hardware.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameServer
{
	internal sealed class CapturableTeamBaseServer
	{
		private PlayerTeamsContainer _playerTeamsContainer;

		public Action<int, List<int>, float> onBaseActivated = delegate
		{
		};

		public Action<int> onBaseCaptureStarted;

		public Action<int> onBaseCaptureStopped;

		private int _teamId;

		private List<int> _attackers = new List<int>();

		private int _numAttackersLastFrame;

		private int _numDefenders;

		private int _numDefendersLastFrame;

		public Vector3 center
		{
			get;
			private set;
		}

		public float radiusSquared
		{
			get;
			private set;
		}

		private int numAttackers => _attackers.Count;

		public CapturableTeamBaseServer(PlayerTeamsContainer playerTeamsContainer, int teamId, Vector3 _center, float _radiusSquared)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			_playerTeamsContainer = playerTeamsContainer;
			_teamId = teamId;
			center = _center;
			radiusSquared = _radiusSquared;
		}

		public void PlayerEntered(int player)
		{
			int playerTeam = _playerTeamsContainer.GetPlayerTeam(TargetType.Player, player);
			if (CanActivateMe(playerTeam))
			{
				_attackers.Add(player);
			}
			else
			{
				_numDefenders++;
			}
		}

		public void EndDetection(float deltaTime)
		{
			if ((FirstCapturerEntered() && !IsBeingDefended()) || (LastDefenderLeft() && IsBeingAttacked()))
			{
				onBaseCaptureStarted(_teamId);
			}
			else if ((LastCapturerLeft() && !IsBeingDefended()) || (FirstDefenderEntered() && IsBeingAttacked()))
			{
				onBaseCaptureStopped(_teamId);
			}
			if (!IsBeingDefended() || !IsBeingAttacked())
			{
				onBaseActivated(_teamId, _attackers, deltaTime);
			}
			_numAttackersLastFrame = numAttackers;
			_attackers.Clear();
			_numDefendersLastFrame = _numDefenders;
			_numDefenders = 0;
		}

		private bool FirstCapturerEntered()
		{
			return _numAttackersLastFrame == 0 && numAttackers > 0;
		}

		private bool CanActivateMe(int playerTeam)
		{
			return playerTeam != _teamId;
		}

		private bool IsBeingDefended()
		{
			return _numDefenders > 0;
		}

		private bool LastCapturerLeft()
		{
			return numAttackers == 0 && _numAttackersLastFrame > 0;
		}

		private bool LastDefenderLeft()
		{
			return _numDefenders == 0 && _numDefendersLastFrame > 0;
		}

		private bool IsBeingAttacked()
		{
			return numAttackers > 0;
		}

		private bool FirstDefenderEntered()
		{
			return _numDefendersLastFrame == 0 && _numDefenders > 0;
		}
	}
}
