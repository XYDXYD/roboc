using Mothership.GarageSkins;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal class MothershipPropPresenter : IInitialize, IMothershipPropPresenter, IWaitForFrameworkDestruction
	{
		private MothershipPropState _propState;

		private bool _premiumState;

		private bool _premiumMothershipEnabledByDefault;

		private bool _megabotState;

		private bool _garageSkinEnabled;

		private MothershipPropActivator _view;

		private MothershipPropType _currentPropMode = MothershipPropType.PropTypePreviewRobotsNonPremium;

		[Inject]
		internal CameraPreview cameraController
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		[Inject]
		internal PremiumMembership premiumMembership
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter LoadingIcon
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory RequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LocalisationWrapper localiseWrapper
		{
			private get;
			set;
		}

		[Inject]
		public GarageSlotOrderPresenter slotOrderPresenter
		{
			private get;
			set;
		}

		[Inject]
		public GarageSlotsPresenter garageSlotsPresenter
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
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal GarageBaySkinNotificationObserver garageBaySkinNotification
		{
			private get;
			set;
		}

		public unsafe void OnDependenciesInjected()
		{
			guiInputController.OnScreenStateChange += OnScreenStateChange;
			cpuPower.RegisterOnCPULoadChanged(OnCPULoadChanged);
			premiumMembership.onSubscriptionActivated += OnPremiumActivated;
			premiumMembership.onSubscriptionExpired += OnPremiumExpired;
			slotOrderPresenter.OnSlotsReordered += UpdateGarageNameAndBay;
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Combine(localiseWrapper.OnLocalisationChanged, new Action(UpdateGarageNameAndBay));
			garagePresenter.OnCurrentGarageNameChange += UpdateGarageNameAndBayStr;
			garageBaySkinNotification.AddAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_propState = MothershipPropState.GaragePreview;
			_premiumState = false;
			_megabotState = false;
			_garageSkinEnabled = false;
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			guiInputController.OnScreenStateChange -= OnScreenStateChange;
			cpuPower.UnregisterOnCPULoadChanged(OnCPULoadChanged);
			premiumMembership.onSubscriptionActivated -= OnPremiumActivated;
			premiumMembership.onSubscriptionExpired -= OnPremiumExpired;
			slotOrderPresenter.OnSlotsReordered -= UpdateGarageNameAndBay;
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Remove(localiseWrapper.OnLocalisationChanged, new Action(UpdateGarageNameAndBay));
			garageBaySkinNotification.RemoveAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public IEnumerator LoadInitialState()
		{
			LoadingIcon.NotifyLoading("PropPresenter");
			ILoadPremiumDataRequest request = RequestFactory.Create<ILoadPremiumDataRequest>();
			TaskService<PremiumInfoData> task = new TaskService<PremiumInfoData>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				LoadingIcon.NotifyLoading("PropPresenter");
			}, delegate
			{
				LoadingIcon.NotifyLoadingDone("PropPresenter");
			}).GetEnumerator();
			LoadingIcon.NotifyLoadingDone("PropPresenter");
			if (task.succeeded)
			{
				PremiumInfoData result = task.result;
				_premiumState = result.HasPremium;
			}
			_premiumMothershipEnabledByDefault = true;
		}

		public MothershipPropState PushThumbnailRenderState()
		{
			MothershipPropState propState = _propState;
			_propState = MothershipPropState.ThumbnailPreview;
			HandleAnyChange();
			return propState;
		}

		public void PopThumbnailRenderState(MothershipPropState prevState)
		{
			_propState = prevState;
			HandleAnyChange();
		}

		private void UpdateGarageNameAndBayStr(string s)
		{
			UpdateGarageNameAndBay();
		}

		private void UpdateGarageNameAndBay()
		{
			_view.SetGarageNameText(garagePresenter.CurrentRobotName);
			_view.SetBayLabelText(garageSlotsPresenter.GetCurrentBayViewIndex());
		}

		private void OnPremiumActivated(TimeSpan t)
		{
			_premiumState = true;
			HandleAnyChange();
		}

		private void OnPremiumExpired()
		{
			_premiumState = false;
			HandleAnyChange();
		}

		public Vector3 GetBayCentre()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return _view.GetFloorCentre();
		}

		private void OnCPULoadChanged(uint currentCpu)
		{
			_megabotState = (currentCpu > cpuPower.MaxCpuPower);
			HandleAnyChange();
		}

		public void SetView(MothershipPropActivator view)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			_view = view;
			_view.SetPropType(_currentPropMode);
			cameraController.SetBayCentre(_view.GetFloorCentre());
		}

		public void SetRobotShopName(string name)
		{
			_view.SetGarageNameText(name);
		}

		private void HandleAnyChange()
		{
			if (_propState == MothershipPropState.ShopOrFactory)
			{
				_view.SetPropType(MothershipPropType.PropTypeCRF);
			}
			else if (_propState == MothershipPropState.GaragePreview)
			{
				if (_garageSkinEnabled)
				{
					_view.SetPropType(MothershipPropType.PropTypePreviewRobotsGarageSkin);
				}
				else if (_premiumState || _premiumMothershipEnabledByDefault)
				{
					_view.SetPropType(MothershipPropType.PropTypePreviewRobotsPremium);
				}
				else
				{
					_view.SetPropType(MothershipPropType.PropTypePreviewRobotsNonPremium);
				}
			}
			else if (_propState == MothershipPropState.BuildMode)
			{
				if (_garageSkinEnabled)
				{
					_view.SetPropType(MothershipPropType.PropTypeEditModeGarageSkin);
				}
				else if (_premiumState || _premiumMothershipEnabledByDefault)
				{
					_view.SetPropType(MothershipPropType.PropTypeEditModePremium);
				}
				else
				{
					_view.SetPropType(MothershipPropType.PropTypeEditModeNonPremium);
				}
			}
			else if (_garageSkinEnabled)
			{
				_view.SetPropType(MothershipPropType.ThumbnailGarageSkinPreview);
			}
			else
			{
				_view.SetPropType(MothershipPropType.ThumbnailGaragePreview);
			}
			_view.SetMegaBayState(_megabotState);
			_view.SetNonMegabotDectoration(!_megabotState && !_garageSkinEnabled);
		}

		private void OnScreenStateChange()
		{
			if (guiInputController.GetActiveScreen() == GuiScreens.RobotShop)
			{
				_propState = MothershipPropState.ShopOrFactory;
			}
			else if (WorldSwitching.IsInGarageMode())
			{
				_propState = MothershipPropState.GaragePreview;
			}
			else
			{
				_propState = MothershipPropState.BuildMode;
			}
			HandleAnyChange();
		}

		private void OnLoadingFailed(ServiceBehaviour behaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(behaviour);
		}

		private void SetGarageSkinMode(ref bool garageSkinActivated)
		{
			_garageSkinEnabled = garageSkinActivated;
			HandleAnyChange();
		}
	}
}
