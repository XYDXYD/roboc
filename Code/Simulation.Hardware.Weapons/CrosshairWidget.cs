using Simulation.Hardware.Weapons.Aeroflak;
using Simulation.Hardware.Weapons.Chaingun;
using Simulation.Hardware.Weapons.Nano;
using Simulation.Hardware.Weapons.RocketLauncher;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class CrosshairWidget : MonoBehaviour, IInitialize
	{
		[SerializeField]
		private LaserCrosshairInfo NormalCrosshair;

		[SerializeField]
		private PlasmaCrosshairInfo PlasmaCrosshair;

		[SerializeField]
		private RailGunCrosshairInfo RailGunCrosshair;

		[SerializeField]
		private NanoWeaponCrosshairInfo NanoWeaponCrosshair;

		[SerializeField]
		private AeroflakCrosshairInfo AeroflakCrosshair;

		[SerializeField]
		private TeslaCrosshairInfo TeslaCrosshair;

		[SerializeField]
		private RocketLauncherCrosshairInfo RocketLauncherCrosshair;

		[SerializeField]
		private RocketLauncherCrosshairInfo SeekerCrosshair;

		[SerializeField]
		private PlasmaCrosshairInfo IonDistorterCrosshair;

		[SerializeField]
		private ChaingunCrosshairInfo chaingunCrosshairInfo;

		[SerializeField]
		private MortarCrosshairInfo mortarCrosshairInfo;

		public GameObject WeaponOfflineGO;

		public GameObject WeaponOfflineFireFailGO;

		public float OffsetAtMinAccuracy = 60f;

		public float HitDuration = 0.2f;

		private CrosshairBase _crosshair;

		private readonly Dictionary<CrosshairType, CrosshairBase> _allCrosshairs = new Dictionary<CrosshairType, CrosshairBase>();

		[Inject]
		internal CrosshairController crosshairController
		{
			private get;
			set;
		}

		[Inject]
		internal ISpectatorModeActivator spectatorModeActivator
		{
			private get;
			set;
		}

		[Inject]
		internal HealthTracker healthTracker
		{
			private get;
			set;
		}

		public CrosshairWidget()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			if (crosshairController != null)
			{
				SetupCrosshairs();
				crosshairController.OnCrosshairTypeChanged += ShowActiveCrosshair;
				healthTracker.HealthChanged += OnCrit;
			}
			if (spectatorModeActivator != null)
			{
				spectatorModeActivator.Register(OnSpectatorMode);
			}
		}

		private void Awake()
		{
			DisableAllCrosshairs();
			NormalCrosshair.CrosshairParent.SetActive(true);
		}

		private void OnEnable()
		{
			if (_crosshair != null)
			{
				_crosshair.ResetAnimations();
			}
		}

		private void OnDestroy()
		{
			if (crosshairController != null)
			{
				crosshairController.OnCrosshairTypeChanged -= ShowActiveCrosshair;
			}
			if (healthTracker != null)
			{
				healthTracker.HealthChanged -= OnCrit;
			}
			if (_crosshair != null)
			{
				_crosshair.Unregister();
			}
			if (spectatorModeActivator != null)
			{
				spectatorModeActivator.Unregister(OnSpectatorMode);
			}
		}

		private void Update()
		{
			if (_crosshair != null)
			{
				_crosshair.UpdateState();
				_crosshair.UpdateAccuracyMotion();
				_crosshair.UpdateDamageIndicator();
			}
		}

		private void OnSpectatorMode(int killer, bool modeEnabled)
		{
			if (modeEnabled)
			{
				_crosshair = null;
				DisableAllCrosshairs();
				WeaponOfflineGO.SetActive(false);
				WeaponOfflineFireFailGO.SetActive(false);
			}
			else
			{
				ShowActiveCrosshair();
			}
		}

		private void ShowActiveCrosshair()
		{
			_crosshair = null;
			CrosshairType crossHairType = crosshairController.GetCrossHairType();
			NormalCrosshair.CrosshairParent.SetActive(crossHairType == CrosshairType.Laser);
			PlasmaCrosshair.CrosshairParent.SetActive(crossHairType == CrosshairType.Plasma);
			RailGunCrosshair.CrosshairParent.SetActive(crossHairType == CrosshairType.Rail);
			NanoWeaponCrosshair.CrosshairParent.SetActive(crossHairType == CrosshairType.Nano);
			AeroflakCrosshair.CrosshairParent.SetActive(crossHairType == CrosshairType.Aeroflak);
			TeslaCrosshair.CrosshairParent.SetActive(crossHairType == CrosshairType.Tesla);
			RocketLauncherCrosshair.CrosshairParent.SetActive(crossHairType == CrosshairType.RocketLauncher);
			SeekerCrosshair.CrosshairParent.SetActive(crossHairType == CrosshairType.Seeker);
			IonDistorterCrosshair.CrosshairParent.SetActive(crossHairType == CrosshairType.IonDistorter);
			chaingunCrosshairInfo.CrosshairParent.SetActive(crossHairType == CrosshairType.Chaingun);
			mortarCrosshairInfo.CrosshairParent.SetActive(crossHairType == CrosshairType.Mortar);
			if (_allCrosshairs.ContainsKey(crossHairType))
			{
				_crosshair = _allCrosshairs[crossHairType];
			}
		}

		private void DisableAllCrosshairs()
		{
			NormalCrosshair.CrosshairParent.SetActive(false);
			PlasmaCrosshair.CrosshairParent.SetActive(false);
			RailGunCrosshair.CrosshairParent.SetActive(false);
			NanoWeaponCrosshair.CrosshairParent.SetActive(false);
			AeroflakCrosshair.CrosshairParent.SetActive(false);
			TeslaCrosshair.CrosshairParent.SetActive(false);
			RocketLauncherCrosshair.CrosshairParent.SetActive(false);
			SeekerCrosshair.CrosshairParent.SetActive(false);
			IonDistorterCrosshair.CrosshairParent.SetActive(false);
			chaingunCrosshairInfo.CrosshairParent.SetActive(false);
			mortarCrosshairInfo.CrosshairParent.SetActive(false);
		}

		private void SetupCrosshairs()
		{
			int num = 11;
			for (int i = 0; i < num; i++)
			{
				CrosshairType crosshairType = (CrosshairType)i;
				CrosshairBase crosshairBase;
				switch (crosshairType)
				{
				case CrosshairType.Laser:
					crosshairBase = new CrosshairBase(NormalCrosshair, NormalCrosshair.MoveablePart, crosshairController, this, OffsetAtMinAccuracy);
					_allCrosshairs.Add(CrosshairType.Laser, crosshairBase);
					break;
				case CrosshairType.Plasma:
					crosshairBase = new CrosshairBase(PlasmaCrosshair, null, crosshairController, this, OffsetAtMinAccuracy);
					_allCrosshairs.Add(CrosshairType.Plasma, crosshairBase);
					break;
				case CrosshairType.Rail:
					crosshairBase = new CrosshairBase(RailGunCrosshair, RailGunCrosshair.MoveablePart, crosshairController, this, RailGunCrosshair.OffsetAtMinAccuracy);
					_allCrosshairs.Add(CrosshairType.Rail, crosshairBase);
					break;
				case CrosshairType.Nano:
					crosshairBase = new CrosshairNanoWeapon(NanoWeaponCrosshair, this, crosshairController, OffsetAtMinAccuracy);
					_allCrosshairs.Add(CrosshairType.Nano, crosshairBase);
					break;
				case CrosshairType.Aeroflak:
					crosshairBase = new CrosshairAeroflak(AeroflakCrosshair, crosshairController, this);
					_allCrosshairs.Add(CrosshairType.Aeroflak, crosshairBase);
					break;
				case CrosshairType.Tesla:
					crosshairBase = new CrosshairBase(TeslaCrosshair, null, crosshairController, this, OffsetAtMinAccuracy);
					_allCrosshairs.Add(CrosshairType.Tesla, crosshairBase);
					break;
				case CrosshairType.RocketLauncher:
					crosshairBase = new CrosshairRocketLauncher(RocketLauncherCrosshair, this, crosshairController, OffsetAtMinAccuracy, RocketLauncherCrosshair);
					_allCrosshairs.Add(CrosshairType.RocketLauncher, crosshairBase);
					break;
				case CrosshairType.Seeker:
					crosshairBase = new CrosshairRocketLauncher(SeekerCrosshair, this, crosshairController, OffsetAtMinAccuracy, SeekerCrosshair);
					_allCrosshairs.Add(CrosshairType.Seeker, crosshairBase);
					break;
				case CrosshairType.IonDistorter:
					crosshairBase = new CrosshairBase(IonDistorterCrosshair, null, crosshairController, this, OffsetAtMinAccuracy);
					_allCrosshairs.Add(CrosshairType.IonDistorter, crosshairBase);
					break;
				case CrosshairType.Chaingun:
					crosshairBase = new CrosshairChaingun(chaingunCrosshairInfo, this, crosshairController, OffsetAtMinAccuracy);
					_allCrosshairs.Add(CrosshairType.Chaingun, crosshairBase);
					break;
				case CrosshairType.Mortar:
					crosshairBase = new CrosshairMortar(mortarCrosshairInfo, crosshairController, this);
					_allCrosshairs.Add(CrosshairType.Mortar, crosshairBase);
					break;
				default:
					throw new Exception("CrosshairWidget not dealing with CrosshairType " + crosshairType);
				}
				crosshairBase.Register();
			}
			if (_crosshair == null)
			{
				ShowActiveCrosshair();
			}
		}

		private void OnCrit(HealthChangeData data)
		{
			if (data.shooterIsMe && data.IsCriticalHit)
			{
				_crosshair.ShowCrit();
			}
		}
	}
}
