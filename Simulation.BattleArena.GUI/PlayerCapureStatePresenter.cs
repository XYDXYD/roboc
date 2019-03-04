using Fabric;
using Simulation.BattleArena.CapturePoint;
using Svelto.Context;
using Svelto.Observer;
using System;

namespace Simulation.BattleArena.GUI
{
	internal sealed class PlayerCapureStatePresenter : IWaitForFrameworkDestruction
	{
		private CapturePointProgressObserver _progressObserver;

		private float[] _captureProgress = new float[3];

		private int _indexToUpdate = -1;

		private CaptureState _currentDisplayed;

		private HUDBattleArenaWidget _view;

		public unsafe PlayerCapureStatePresenter(CapturePointProgressObserver _progressObserver)
		{
			this._progressObserver = _progressObserver;
			_progressObserver.AddAction(new ObserverAction<TeamBaseStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_progressObserver.RemoveAction(new ObserverAction<TeamBaseStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		internal void RegisterView(HUDBattleArenaWidget view)
		{
			_view = view;
			view.InitHudWidget();
		}

		internal void ShowView(bool show, CaptureState state, int id)
		{
			_indexToUpdate = id;
			if (show)
			{
				_view.sliderContainer.SetActive(state != CaptureState.contesting);
				if (!_view.get_gameObject().get_activeSelf())
				{
					_view.SetHealthPercent(_captureProgress[_indexToUpdate], animate: false);
					EventManager.get_Instance().SetParameter("HUD_CaptureBar_Loop", "Fill_Amount", _captureProgress[_indexToUpdate], _view.get_gameObject());
				}
				if (_currentDisplayed != state)
				{
					string captureStateString = GetCaptureStateString(state);
					_view.SetExtraInfoLabel(captureStateString);
					_currentDisplayed = state;
				}
			}
			_view.Show(show);
		}

		private string GetCaptureStateString(CaptureState captureState)
		{
			string key = (captureState != CaptureState.capturing) ? "strContested" : "strCapturing";
			return StringTableBase<StringTable>.Instance.GetString(key);
		}

		private void HandleOnCapturePointProgressChanged(ref TeamBaseStateDependency parameter)
		{
			_captureProgress[parameter.team] = parameter.currentProgress / parameter.maxProgress;
			if (parameter.team == _indexToUpdate && _currentDisplayed != CaptureState.contesting)
			{
				_view.SetHealthPercent(_captureProgress[_indexToUpdate]);
				EventManager.get_Instance().SetParameter("HUD_CaptureBar_Loop", "Fill_Amount", _captureProgress[_indexToUpdate], _view.get_gameObject());
			}
		}
	}
}
