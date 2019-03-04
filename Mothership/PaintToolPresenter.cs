using Services.Requests.Interfaces;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class PaintToolPresenter : IInitialize, IWaitForFrameworkInitialization, IHandleEditingInput, IInputComponent, IComponent
	{
		private bool _hasPremium;

		private ColorPaletteData _colorPalette;

		private PlatformConfigurationSettings _platformConfigSettings;

		private ServiceBehaviour _serviceErrorBehaviour;

		[Inject]
		public ICubeHolder cubeHolder
		{
			private get;
			set;
		}

		[Inject]
		public PremiumMembership premiumMembership
		{
			private get;
			set;
		}

		[Inject]
		public IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		public PaintColorSelectorDisplay colorSelectorDisplay
		{
			private get;
			set;
		}

		[Inject]
		public CurrentToolMode toolMode
		{
			private get;
			set;
		}

		[Inject]
		public PaintFillController fillController
		{
			private get;
			set;
		}

		public PaintToolView paintToolView
		{
			private get;
			set;
		}

		public DisplayCubePainter displayCubePainter
		{
			private get;
			set;
		}

		public int maxColours => _colorPalette.Count;

		public event Action<int> PaintWheelScrolled = delegate
		{
		};

		public event Action<int> PaintWheelBlockedFromScrolling = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			premiumMembership.onSubscriptionActivated += OnPremiumActivated;
			premiumMembership.onSubscriptionExpired += OnPremiumExpired;
			LoadColorPalette();
			LoadPlatformConfig();
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			if (_serviceErrorBehaviour != null)
			{
				if (_serviceErrorBehaviour.exceptionThrown != null)
				{
					Debug.LogException(_serviceErrorBehaviour.exceptionThrown);
				}
				else
				{
					Debug.LogException(new Exception("ServiceBehaviour returned from ILoadDefaultColorPaletteRequest was not null"));
				}
				ErrorWindow.ShowServiceErrorWindow(_serviceErrorBehaviour);
				_serviceErrorBehaviour = null;
				return;
			}
			cubeHolder.currentPaletteId = 0;
			cubeHolder.currentColor = _colorPalette[0];
			if (paintToolView != null)
			{
				paintToolView.SetCurrentPalette(_colorPalette);
				this.PaintWheelScrolled(_colorPalette.GetVisualIndexFromColorIndex(cubeHolder.currentPaletteId));
				cubeHolder.currentColor = _colorPalette[cubeHolder.currentPaletteId];
				paintToolView.SetCurrentColor(cubeHolder.currentPaletteId);
				paintToolView.SetShopVisibility(_platformConfigSettings.MainShopButtonAvailable);
			}
		}

		public void SetHUDActive(bool active)
		{
			paintToolView.SetActive(active);
			HandlePremiumLock(_hasPremium);
		}

		public bool IsHUDActive()
		{
			return paintToolView.IsActive();
		}

		public void OnDestroy()
		{
			premiumMembership.onSubscriptionActivated -= OnPremiumActivated;
			premiumMembership.onSubscriptionExpired -= OnPremiumExpired;
		}

		public void OnColorSelectedFromSelectionWindow(byte slotNum)
		{
			colorSelectorDisplay.NewColorSelected();
			TrySetCurrentColor(slotNum);
		}

		private void LoadColorPalette()
		{
			ILoadDefaultColorPaletteRequest loadDefaultColorPaletteRequest = serviceFactory.Create<ILoadDefaultColorPaletteRequest>();
			loadDefaultColorPaletteRequest.SetAnswer(new ServiceAnswer<ColorPaletteData>(delegate(ColorPaletteData defaultPalette)
			{
				_colorPalette = defaultPalette;
			}, delegate(ServiceBehaviour behaviour)
			{
				_serviceErrorBehaviour = behaviour;
			}));
			loadDefaultColorPaletteRequest.Execute();
		}

		private void LoadPlatformConfig()
		{
			ILoadPlatformConfigurationRequest request = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
			TaskService<PlatformConfigurationSettings> taskService = request.AsTask();
			taskService.Execute();
			_platformConfigSettings = taskService.result;
		}

		private void OnPremiumActivated(TimeSpan t)
		{
			_hasPremium = true;
			HandlePremiumLock(_hasPremium);
		}

		private void OnPremiumExpired()
		{
			_hasPremium = false;
			HandlePremiumLock(_hasPremium);
			if (cubeHolder.currentColor.isPremium)
			{
				TrySetCurrentColor(0);
			}
		}

		private void HandlePremiumLock(bool hasPremium)
		{
			if (paintToolView.IsActive())
			{
				paintToolView.SetPremiumLocked(hasPremium);
			}
		}

		public void HandleEditingInput(InputEditingData data)
		{
			if (Object.op_Implicit(displayCubePainter) && toolMode.currentBuildTool == CurrentToolMode.ToolMode.Paint && displayCubePainter.get_isActiveAndEnabled() && !fillController.painting && data[EditingInputAxis.ROTATE_CUBE] != 0f)
			{
				ScrollColors(data[EditingInputAxis.ROTATE_CUBE]);
			}
		}

		private void ScrollColors(float inputAxisValue)
		{
			int visualIndexFromColorIndex = _colorPalette.GetVisualIndexFromColorIndex(cubeHolder.currentPaletteId);
			int num = visualIndexFromColorIndex + ((inputAxisValue < 0f) ? 1 : (-1));
			if (num < 0)
			{
				num = _colorPalette.Count - 1;
			}
			else if (num >= _colorPalette.Count)
			{
				num = 0;
			}
			TrySetCurrentColor(_colorPalette.GetFromVisualIndex(num).paletteIndex);
		}

		public void TrySetCurrentColor(byte slotNum)
		{
			int visualIndexFromColorIndex = _colorPalette.GetVisualIndexFromColorIndex(slotNum);
			if (cubeHolder.currentPaletteId == slotNum || (!_hasPremium && _colorPalette[slotNum].isPremium))
			{
				this.PaintWheelBlockedFromScrolling(visualIndexFromColorIndex);
				return;
			}
			cubeHolder.currentPaletteId = slotNum;
			cubeHolder.currentColor = _colorPalette[slotNum];
			paintToolView.SetCurrentColor(slotNum);
			this.PaintWheelScrolled(_colorPalette.GetVisualIndexFromColorIndex(slotNum));
		}
	}
}
