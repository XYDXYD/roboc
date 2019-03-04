using Mothership;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using UnityEngine;

internal sealed class GarageCameraOrientationController : IInitialize, IWaitForFrameworkDestruction
{
	private const float IDLE_PAN_VELOCITY = 1f;

	private const float RESUME_PANNING_AFTER_TIME = 60f;

	private ITaskRoutine _panCameraTask;

	private ITaskRoutine _resumeCameraTask;

	private bool _paused;

	private CameraDraggingView _view;

	[Inject]
	internal IGUIInputControllerMothership guiInputController
	{
		private get;
		set;
	}

	[Inject]
	internal IRobotShopController robotShopController
	{
		private get;
		set;
	}

	[Inject]
	internal GaragePresenter garagePresenter
	{
		private get;
		set;
	}

	[Inject]
	internal CameraPreview cameraController
	{
		private get;
		set;
	}

	[Inject]
	internal RobotShopSubmissionController robotSubmissionController
	{
		private get;
		set;
	}

	void IInitialize.OnDependenciesInjected()
	{
		guiInputController.OnScreenStateChange += OnScreenStateChange;
		RobotShopSubmissionController robotSubmissionController = this.robotSubmissionController;
		robotSubmissionController.OnRobotSubmissionViewStateChanged = (Action<bool>)Delegate.Combine(robotSubmissionController.OnRobotSubmissionViewStateChanged, new Action<bool>(OnSubmissionViewStateChange));
		garagePresenter.OnCurrentGarageSlotChanged += HandleOnGarageSlotChange;
		_panCameraTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)PanCameraTask);
		_panCameraTask.Start((Action<PausableTaskException>)null, (Action)null);
		_resumeCameraTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)ResumeCameraTask);
		_resumeCameraTask.Stop();
		_paused = false;
	}

	public void SetView(CameraDraggingView view)
	{
		_view = view;
	}

	private void ScheduleResume()
	{
		if (!_paused)
		{
			_resumeCameraTask.Start((Action<PausableTaskException>)null, (Action)null);
		}
	}

	public void OnUserPanCamera(Vector2 panAmount)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		ScheduleResume();
		_panCameraTask.Pause();
		_paused = true;
		cameraController.MoveView(panAmount);
	}

	void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		guiInputController.OnScreenStateChange -= OnScreenStateChange;
		RobotShopSubmissionController robotSubmissionController = this.robotSubmissionController;
		robotSubmissionController.OnRobotSubmissionViewStateChanged = (Action<bool>)Delegate.Remove(robotSubmissionController.OnRobotSubmissionViewStateChanged, new Action<bool>(OnSubmissionViewStateChange));
		garagePresenter.OnCurrentGarageSlotChanged -= HandleOnGarageSlotChange;
	}

	private void HandleOnGarageSlotChange(GarageSlotDependency currentGarageSlot)
	{
		if (ShouldRotate())
		{
			_panCameraTask.Resume();
			_paused = false;
		}
	}

	private void OnSubmissionViewStateChange(bool isVisible)
	{
		if (isVisible)
		{
			_panCameraTask.Pause();
			_paused = true;
		}
		else
		{
			_panCameraTask.Resume();
			_paused = false;
		}
	}

	private void OnScreenStateChange()
	{
		if (ShouldRotate())
		{
			_panCameraTask.Resume();
			_paused = false;
		}
		else
		{
			_resumeCameraTask.Stop();
			_panCameraTask.Pause();
			_paused = true;
		}
	}

	private bool ShouldRotate()
	{
		GuiScreens activeScreen = guiInputController.GetActiveScreen();
		return activeScreen == GuiScreens.Garage || activeScreen == GuiScreens.RobotShop || activeScreen == GuiScreens.PlayScreen || activeScreen == GuiScreens.InventoryScreen;
	}

	private IEnumerator ResumeCameraTask()
	{
		yield return (object)new WaitForSecondsEnumerator(60f);
		if (_paused && ShouldRotate())
		{
			_paused = false;
			_panCameraTask.Resume();
		}
	}

	private IEnumerator PanCameraTask()
	{
		while (true)
		{
			cameraController.MoveView(new Vector2(1f, 0f));
			yield return null;
		}
	}
}
