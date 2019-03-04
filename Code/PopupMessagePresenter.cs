using Svelto.IoC;
using Svelto.Observer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;

internal sealed class PopupMessagePresenter : IInitialize
{
	private bool _tutorialWasHidden;

	private bool _popupsRegistered;

	private PopupMessageView _view;

	private InvalidPlacementObserver _popupMessageObserver;

	private TutorialTipObserver _tutorialTipObserver;

	private TutorialMessageData _pendingTutorialMessageData;

	private TutorialMessageData _currentTutorialMessageData;

	private ITaskRoutine _animateViewTask;

	private Dictionary<InvalidPlacementType, PopupMessageInfo> _registeredPopups = new Dictionary<InvalidPlacementType, PopupMessageInfo>();

	[Inject]
	internal IGUIInputController guiController
	{
		private get;
		set;
	}

	[Inject]
	internal ICubeHolder selectedCube
	{
		private get;
		set;
	}

	public unsafe PopupMessagePresenter(InvalidPlacementObserver popupMessageObserver, TutorialTipObserver tutorialtipObserver)
	{
		_animateViewTask = TaskRunner.get_Instance().AllocateNewTaskRoutine();
		_popupMessageObserver = popupMessageObserver;
		_tutorialTipObserver = tutorialtipObserver;
		if (_popupMessageObserver != null)
		{
			_popupMessageObserver.AddAction(new ObserverAction<InvalidPlacementType>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}
		if (_tutorialTipObserver != null)
		{
			_tutorialTipObserver.AddAction(new ObserverAction<TutorialMessageData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}
	}

	void IInitialize.OnDependenciesInjected()
	{
		guiController.OnScreenStateChange += HandleOnScreenStateChange;
	}

	private void HandleOnScreenStateChange()
	{
		if (!WorldSwitching.IsInBuildMode())
		{
			_view.Hide();
		}
	}

	private void ShowWarningMessage(ref InvalidPlacementType parameter)
	{
		if (!_registeredPopups.TryGetValue(parameter, out PopupMessageInfo value))
		{
			return;
		}
		string replaceStringWithInputActionKeyMap = StringTableBase<StringTable>.Instance.GetReplaceStringWithInputActionKeyMap(value.Text);
		if (value.stringOverridesByCubeType.Length > 0)
		{
			CubeTypeID selectedCubeID = selectedCube.selectedCubeID;
			for (int i = 0; i < value.stringOverridesByCubeType.Length; i++)
			{
				if (value.stringOverridesByCubeType[i].stringKeyAsCubeTypeID == selectedCubeID)
				{
					replaceStringWithInputActionKeyMap = StringTableBase<StringTable>.Instance.GetReplaceStringWithInputActionKeyMap(value.stringOverridesByCubeType[i].stringToShow);
				}
			}
		}
		_view.SetupMessage(replaceStringWithInputActionKeyMap, PopupMessageCategory.Warning);
		_view.Show();
		StartNewAnimateViewTask(AnimateView(value.VisibleTime, PopupMessageCategory.Warning));
	}

	private void ShowTutorialMessage(ref TutorialMessageData parameters)
	{
		_currentTutorialMessageData = parameters;
		if (_currentTutorialMessageData.justHideExistingMessages && _view.IsActive())
		{
			_tutorialWasHidden = true;
			StartNewAnimateViewTask(AnimateViewImmediately());
		}
		if (!_view.IsActive())
		{
			string replaceStringWithInputActionKeyMap = StringTableBase<StringTable>.Instance.GetReplaceStringWithInputActionKeyMap(_currentTutorialMessageData.text);
			_view.SetupMessage(replaceStringWithInputActionKeyMap, PopupMessageCategory.Tutorial);
			_view.Show();
			StartNewAnimateViewTask(AnimateView(_currentTutorialMessageData.timeToShow, PopupMessageCategory.Tutorial));
		}
		else if (!_currentTutorialMessageData.justHideExistingMessages)
		{
			_pendingTutorialMessageData = new TutorialMessageData(_currentTutorialMessageData.text, _currentTutorialMessageData.timeToShow, hide_: false);
			StartNewAnimateViewTask(AnimateViewImmediately());
		}
	}

	private void StartNewAnimateViewTask(IEnumerator task)
	{
		if (_animateViewTask != null)
		{
			_animateViewTask.SetEnumerator(task);
			_animateViewTask.Start((Action<PausableTaskException>)null, (Action)null);
		}
	}

	private IEnumerator AnimateViewImmediately()
	{
		_view.PlayAnimationOut();
		yield return (object)new WaitForSecondsEnumerator(0.5f);
		_view.Hide();
		if (_pendingTutorialMessageData != null)
		{
			ShowTutorialMessage(ref _pendingTutorialMessageData);
		}
	}

	private IEnumerator AnimateView(float visibleTime, PopupMessageCategory category)
	{
		_tutorialWasHidden = false;
		_view.PlayAnimationIn();
		if (visibleTime != 0f)
		{
			yield return (object)new WaitForSecondsEnumerator(visibleTime);
			if (!_tutorialWasHidden)
			{
				_view.PlayAnimationOut();
			}
			if (!_tutorialWasHidden)
			{
				yield return (object)new WaitForSecondsEnumerator(0.5f);
			}
			if (!_tutorialWasHidden)
			{
				_view.Hide();
			}
		}
		if (category == PopupMessageCategory.Warning && _currentTutorialMessageData != null)
		{
			ShowTutorialMessage(ref _currentTutorialMessageData);
		}
	}

	internal void SetView(PopupMessageView popupMessageView)
	{
		_view = popupMessageView;
		if (_popupsRegistered)
		{
			_view.Hide();
		}
	}

	internal void RegisterData(PopupMessageInfo[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			_registeredPopups.Add(data[i].type, data[i]);
		}
		_popupsRegistered = true;
		if (_view != null)
		{
			_view.Hide();
		}
	}
}
