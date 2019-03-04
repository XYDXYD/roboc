using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal class PlayerListHUDWidget_NewMeta : MonoBehaviour
	{
		public UILabel nameLabel;

		public UILabel respawnTimeLabel;

		public UISlider healthSlider;

		public UITexture AvatarTexture;

		public UITexture ClanAvatarTexture;

		private string _playerDisplayName;

		private float _respawnTimeRemaining;

		private int _lastRespawnTimeSeconds;

		private bool _isLocalPlayer;

		private bool _isInMyPlatoon;

		public string PlayerDisplayName => _playerDisplayName;

		public PlayerListHUDWidget_NewMeta()
			: this()
		{
		}

		public void Initialise(string name, bool isLocalPlayer, bool isEnemy, bool isInMyPlatoon)
		{
			_playerDisplayName = name;
			nameLabel.set_text(name);
			respawnTimeLabel.set_text(string.Empty);
			_isLocalPlayer = isLocalPlayer;
			_isInMyPlatoon = isInMyPlatoon;
			MarkAsDead();
			AvatarTexture.get_gameObject().SetActive(true);
		}

		public void Respawn()
		{
			_respawnTimeRemaining = 0f;
			SetCurrentHealthPercent(1f);
			SetColour(isDead: false);
		}

		public void RespawnScheduled(int timeSeconds)
		{
			_respawnTimeRemaining = timeSeconds;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)RespawnCountdown);
		}

		public void MarkAsDead()
		{
			SetCurrentHealthPercent(0f);
			SetColour(isDead: true);
		}

		public void SetCurrentHealthPercent(float percent)
		{
			healthSlider.set_value(percent);
		}

		public void ResetTimer()
		{
			_respawnTimeRemaining = 0f;
		}

		private void Start()
		{
		}

		private IEnumerator RespawnCountdown()
		{
			while (_respawnTimeRemaining > 0f)
			{
				_respawnTimeRemaining = Mathf.Max(_respawnTimeRemaining - Time.get_deltaTime(), 0f);
				int timeSeconds = Mathf.CeilToInt(_respawnTimeRemaining);
				if (timeSeconds != _lastRespawnTimeSeconds)
				{
					_lastRespawnTimeSeconds = timeSeconds;
					respawnTimeLabel.set_text(timeSeconds.ToString());
				}
				yield return null;
			}
			respawnTimeLabel.set_text(string.Empty);
		}

		public void SetColour(bool isDead)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			if (_isLocalPlayer)
			{
				nameLabel.set_color(PlayerColours.playerColor);
			}
			else if (isDead)
			{
				nameLabel.set_color(PlayerColours.deadColor);
			}
			else if (_isInMyPlatoon)
			{
				nameLabel.set_color(PlayerColours.platoonColor);
			}
			else
			{
				nameLabel.set_color(PlayerColours.enemyColor);
			}
		}

		private void HideEnemyDetails()
		{
			healthSlider.get_gameObject().SetActive(false);
		}
	}
}
