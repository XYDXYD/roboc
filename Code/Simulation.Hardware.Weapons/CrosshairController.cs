using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class CrosshairController : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		public float angleToHorizonDeg;

		public float weaponSpinPower;

		private FasterList<CrosshairBase> _crosshairs = new FasterList<CrosshairBase>();

		private FasterList<CrosshairType> _machineCrosshairTypes = new FasterList<CrosshairType>();

		private bool _isZoomed;

		private CrosshairType _currenCrosshairType;

		[Inject]
		public MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		public ZoomEngine zoomMode
		{
			private get;
			set;
		}

		[Inject]
		public SwitchWeaponObserver switchWeaponObserver
		{
			private get;
			set;
		}

		public Vector3 lockTargetPosition
		{
			get;
			set;
		}

		public int lockStage
		{
			get;
			set;
		}

		public bool inRange
		{
			get;
			set;
		}

		public event Action OnCrosshairTypeChanged = delegate
		{
		};

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			zoomMode.OnZoomModeChange -= OnZoomChange;
			switchWeaponObserver.SwitchCrosshairEvent -= OnSwitchCrosshair;
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			zoomMode.OnZoomModeChange += OnZoomChange;
			switchWeaponObserver.SwitchCrosshairEvent += OnSwitchCrosshair;
		}

		public void RegisterCrosshair(CrosshairBase crosshair)
		{
			_crosshairs.Add(crosshair);
		}

		public void UnRegisterCrosshair(CrosshairBase crosshair)
		{
			_crosshairs.Remove(crosshair);
		}

		public void ShowDamageEffect(int stackCountPercent)
		{
			for (int i = 0; i < _crosshairs.get_Count(); i++)
			{
				_crosshairs.get_Item(i).ShowDamageEffect(stackCountPercent);
			}
		}

		public void ActivateNoFireState(bool active)
		{
			for (int i = 0; i < _crosshairs.get_Count(); i++)
			{
				_crosshairs.get_Item(i).ActivateNoFireState(active);
			}
		}

		public void ActivateGroundWarning(bool active)
		{
			for (int i = 0; i < _crosshairs.get_Count(); i++)
			{
				_crosshairs.get_Item(i).ActivateGroundWarning(active);
			}
		}

		public void SetWeaponAccuracy(float accuracy)
		{
			for (int i = 0; i < _crosshairs.get_Count(); i++)
			{
				CrosshairBase crosshairBase = _crosshairs.get_Item(i);
				crosshairBase.accuracy = accuracy;
			}
		}

		public CrosshairType GetCrossHairType()
		{
			return _currenCrosshairType;
		}

		public FasterList<CrosshairType> GetCrossHairTypes()
		{
			return _machineCrosshairTypes;
		}

		public bool IsZoomed()
		{
			return _isZoomed;
		}

		private void OnZoomChange(ZoomType zoomType, float zoomAmount)
		{
			_isZoomed = (zoomType == ZoomType.Zoomed);
		}

		private void OnSwitchCrosshair(CrosshairType crosshairType)
		{
			_currenCrosshairType = crosshairType;
			this.OnCrosshairTypeChanged();
		}
	}
}
