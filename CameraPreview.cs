using Mothership;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

internal sealed class CameraPreview : IInitialize, IWaitForFrameworkDestruction
{
	private CameraPreviewView _view;

	private bool _reframeRequired;

	private CameraPreviewUtility _cameraPreviewUtility;

	[Inject]
	internal IGUIInputControllerMothership guiInputController
	{
		private get;
		set;
	}

	[Inject]
	internal RobotDimensionChangedObserver robotDimensionChangedObs
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

	unsafe void IInitialize.OnDependenciesInjected()
	{
		_reframeRequired = false;
		_cameraPreviewUtility = new CameraPreviewUtility();
		guiInputController.OnScreenStateChange += OnScreenStateChange;
		garagePresenter.OnCurrentGarageSlotChanged += HandleOnGarageSlotChange;
		robotDimensionChangedObs.AddAction(new ObserverAction<RobotDimensionDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		guiInputController.OnScreenStateChange -= OnScreenStateChange;
		robotDimensionChangedObs.RemoveAction(new ObserverAction<RobotDimensionDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		garagePresenter.OnCurrentGarageSlotChanged -= HandleOnGarageSlotChange;
	}

	private void HandleOnGarageSlotChange(GarageSlotDependency currentGarageSlot)
	{
		if (guiInputController.GetActiveScreen() != GuiScreens.BuildMode)
		{
			HandleCameraDataChanged();
		}
	}

	private void OnScreenStateChange()
	{
		if (_cameraPreviewUtility.GetConfig().HasValue && !(Camera.get_main() == null))
		{
			_cameraPreviewUtility.UpdateCameraFOV(Camera.get_main(), CanUpdateCamera());
		}
	}

	private bool CanUpdateCamera()
	{
		return guiInputController.GetActiveScreen() == GuiScreens.Garage || guiInputController.GetActiveScreen() == GuiScreens.RobotShop || guiInputController.GetActiveScreen() == GuiScreens.PlayScreen || guiInputController.GetActiveScreen() == GuiScreens.InventoryScreen || guiInputController.GetActiveScreen() == GuiScreens.PrebuiltRobotScreen;
	}

	private void OnMachineSizeSet(ref RobotDimensionDependency dependency)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		_cameraPreviewUtility.SetMachineSize(dependency.minBounds, dependency.maxBounds);
		_reframeRequired = true;
		if (CanUpdateCamera())
		{
			HandleCameraDataChanged();
		}
	}

	public void SetBayCentre(Vector3 bayCentre)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_cameraPreviewUtility.SetCentre(bayCentre);
		HandleCameraDataChanged();
	}

	public void SetConfig(CameraPreviewUtility.CameraConfiguration config)
	{
		_cameraPreviewUtility.SetConfig(config);
		HandleCameraDataChanged();
	}

	public void SetView(CameraPreviewView newView)
	{
		_view = newView;
		HandleCameraDataChanged();
	}

	private void HandleCameraDataChanged()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		if (!(_view != null) || !_cameraPreviewUtility.GetConfig().HasValue || !_cameraPreviewUtility.GetCentre().HasValue || !_cameraPreviewUtility.GetMachineBoundsMax().HasValue || !_cameraPreviewUtility.GetMachineBoundsMin().HasValue)
		{
			return;
		}
		Vector3 calculatedPosition;
		float currentDistanceFromCameraTarget = _cameraPreviewUtility.RecalculateCameraPosition(Camera.get_main(), out calculatedPosition);
		Camera.get_main().get_transform().set_position(calculatedPosition);
		Camera.get_main().get_transform().LookAt(_cameraPreviewUtility.CalculateCameraTarget());
		if (_reframeRequired)
		{
			_reframeRequired = false;
			_cameraPreviewUtility.ReFrameRobotInMiddleOfCamera(Camera.get_main(), currentDistanceFromCameraTarget);
			if (guiInputController.GetActiveScreen() == GuiScreens.RobotShop || guiInputController.GetActiveScreen() == GuiScreens.PlayScreen || guiInputController.GetActiveScreen() == GuiScreens.InventoryScreen || guiInputController.GetActiveScreen() == GuiScreens.PrebuiltRobotScreen)
			{
				_cameraPreviewUtility.RecalculateCameraPosition(Camera.get_main(), out calculatedPosition);
				Camera.get_main().get_transform().set_position(calculatedPosition);
				Camera.get_main().get_transform().LookAt(_cameraPreviewUtility.CalculateCameraTarget());
			}
		}
		_cameraPreviewUtility.UpdateCameraFOV(Camera.get_main(), revertToNormal: false);
	}

	public void MoveView(Vector2 value)
	{
		if (_cameraPreviewUtility.GetConfig().HasValue)
		{
			float currentPitch = _cameraPreviewUtility.GetCurrentPitch();
			float num;
			if (value.y < 0f)
			{
				CameraPreviewUtility.CameraConfiguration value2 = _cameraPreviewUtility.GetConfig().Value;
				num = value2.pitchMax;
			}
			else
			{
				CameraPreviewUtility.CameraConfiguration value3 = _cameraPreviewUtility.GetConfig().Value;
				num = value3.pitchMin;
			}
			float num2 = Mathf.Abs(value.y) * Time.get_deltaTime();
			CameraPreviewUtility.CameraConfiguration value4 = _cameraPreviewUtility.GetConfig().Value;
			float currentPitch2 = Mathf.MoveTowards(currentPitch, num, num2 * value4.draggingRotationSpeed);
			_cameraPreviewUtility.SetCurrentPitch(currentPitch2);
			float currentYaw = _cameraPreviewUtility.GetCurrentYaw();
			float num3 = currentYaw;
			float num4 = value.x * Time.get_deltaTime();
			CameraPreviewUtility.CameraConfiguration value5 = _cameraPreviewUtility.GetConfig().Value;
			currentYaw = num3 + num4 * value5.draggingRotationSpeed;
			while (360f < currentYaw)
			{
				currentYaw -= 360f;
			}
			for (; currentYaw < 0f; currentYaw += 360f)
			{
			}
			_cameraPreviewUtility.SetCurrentYaw(currentYaw);
			HandleCameraDataChanged();
		}
	}

	public void ZoomView(float value)
	{
		float num = value * Time.get_deltaTime();
		CameraPreviewUtility.CameraConfiguration value2 = _cameraPreviewUtility.GetConfig().Value;
		float num2 = num * value2.zoomSpeed;
		float desiredZoomAmount = _cameraPreviewUtility.GetDesiredZoomAmount();
		desiredZoomAmount -= num2;
		if (desiredZoomAmount < 0.01f)
		{
			desiredZoomAmount = 0.01f;
		}
		if (desiredZoomAmount > 1f)
		{
			desiredZoomAmount = 1f;
		}
		_cameraPreviewUtility.SetDesiredZoomAmount(desiredZoomAmount);
		HandleCameraDataChanged();
	}
}
