using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal sealed class LobbyGameStartPresenter
	{
		public Action OnInitialLobbyGuiClose = delegate
		{
		};

		private LobbyGameStartView _view;

		private float _time;

		private float _lastTime;

		private bool _countingDown;

		public bool hasBeenClosed
		{
			get;
			private set;
		}

		public LobbyGameStartPresenter()
		{
			hasBeenClosed = (WorldSwitching.GetGameModeType() == GameModeType.TestMode);
		}

		public void SetView(LobbyGameStartView view)
		{
			_view = view;
			view.get_gameObject().SetActive(WorldSwitching.IsMultiplayer());
		}

		private IEnumerator Tick()
		{
			while (_countingDown && !hasBeenClosed)
			{
				float realDeltaTime = Time.get_realtimeSinceStartup() - _lastTime;
				_time -= realDeltaTime;
				_time = Mathf.Max(_time, 0f);
				_view.SetTimer(_time);
				_lastTime = Time.get_realtimeSinceStartup();
				yield return null;
			}
		}

		public void OnGameStart()
		{
			if (OnInitialLobbyGuiClose != null)
			{
				Console.BatchLog = true;
				Console.Log("Game Started");
				if (_view != null)
				{
					_view.Close();
				}
				hasBeenClosed = true;
				OnInitialLobbyGuiClose();
				OnInitialLobbyGuiClose = null;
			}
		}

		public void OnPitGameStart()
		{
			if (_view != null)
			{
				_view.Close();
			}
			hasBeenClosed = true;
		}

		public void SetTimer(float seconds)
		{
			if (!_countingDown)
			{
				_countingDown = true;
				if (_view != null)
				{
					TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
				}
			}
			_time = seconds;
			_lastTime = Time.get_realtimeSinceStartup();
			if (_time <= 0f)
			{
				_countingDown = false;
			}
		}
	}
}
