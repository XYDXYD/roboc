using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using UnityEngine;

public class CursorModeManager : MonoBehaviour
{
	private bool _lockCursor;

	private ITaskRoutine _lockIt;

	private ITaskRoutine _confineIt;

	public bool lockCursor
	{
		private get
		{
			return _lockCursor;
		}
		set
		{
			_lockCursor = value;
			SetCursor(_lockCursor);
		}
	}

	public CursorModeManager()
		: this()
	{
	}

	private void Awake()
	{
		_lockIt = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)(() => Confined(mode: true)));
		_confineIt = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)(() => Confined(mode: false)));
	}

	private void SetCursor(bool lockIT)
	{
		_lockIt.Stop();
		_confineIt.Stop();
		if (lockIT)
		{
			_lockIt.Start((Action<PausableTaskException>)null, (Action)null);
		}
		else
		{
			_confineIt.Start((Action<PausableTaskException>)null, (Action)null);
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			SetCursor(_lockCursor);
		}
		else
		{
			SetCursor(lockIT: true);
		}
	}

	private IEnumerator Confined(bool mode)
	{
		Cursor.set_lockState(0);
		yield return (object)new WaitForSecondsEnumerator(0.1f);
		Cursor.set_lockState(mode ? 1 : 2);
		Cursor.set_visible(!mode);
	}
}
