using Fabric;
using Simulation.DeathEffects;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;
using Xft;

namespace Simulation.Hardware.Weapons
{
	internal sealed class ZoomEngine : MultiEntityViewsEngine<ZoomNode, MachineInputNode, MachineInvisibilityNode, CameraDeathAnimationEntityView>, IInitialize, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private readonly Dictionary<ItemDescriptor, ZoomNode> _weaponCategoryZoomMode = new Dictionary<ItemDescriptor, ZoomNode>();

		private readonly Dictionary<int, ZoomNode> _allWeapons = new Dictionary<int, ZoomNode>();

		private int _machineId;

		private ItemDescriptor _currentActiveSubcategory;

		private float _defaultFoV = 60f;

		private float _zoomedFoV = 60f;

		private ZoomType _zoom;

		private bool _canZoom;

		private float _zoomAmount = 1f;

		private bool _toggleZoom;

		private bool _lastInput;

		private FasterList<Renderer> _playerRobotRenderers;

		private FasterList<UIWidget> _playerRobotWidgets;

		private PitLeaderFX _playerRobotPitLeaderEffects;

		private XWeaponTrail[] _playerFlagTrails;

		private Camera[] _cameras;

		private bool _isPlayerSpectator;

		private bool _isPlayerCloaked;

		[Inject]
		internal SwitchWeaponObserver switchWeaponObserver
		{
			get;
			set;
		}

		[Inject]
		internal MouseSettings mouseSettings
		{
			get;
			set;
		}

		[Inject]
		internal ISpectatorModeActivator spectatorModeActivator
		{
			private get;
			set;
		}

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		public float sqrDrawDistanceScale
		{
			get;
			private set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		private bool zoomed => _zoom == ZoomType.Zoomed;

		public event Action<ZoomType, float> OnZoomModeChange = delegate
		{
		};

		public void Ready()
		{
		}

		protected override void Add(ZoomNode node)
		{
			if (!node.ownerComponent.ownedByMe)
			{
				return;
			}
			_machineId = node.ownerComponent.machineId;
			_allWeapons.Add(node.get_ID(), node);
			ItemDescriptor itemDescriptor = node.itemDescriptorComponent.itemDescriptor;
			if (!_weaponCategoryZoomMode.ContainsKey(itemDescriptor))
			{
				_weaponCategoryZoomMode.Add(itemDescriptor, node);
				if (itemDescriptor.Equals(_currentActiveSubcategory))
				{
					_canZoom = node.zoomComponent.canZoom;
					_zoomedFoV = node.zoomComponent.zoomedFov;
				}
			}
		}

		protected override void Remove(ZoomNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				_allWeapons.Remove(node.get_ID());
				ItemDescriptor itemDescriptor = node.itemDescriptorComponent.itemDescriptor;
				_weaponCategoryZoomMode.Remove(itemDescriptor);
			}
		}

		protected override void Add(MachineInputNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				node.machineInput.zoomPressed.subscribers += OnZoomPressed;
			}
		}

		protected override void Remove(MachineInputNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				node.machineInput.zoomPressed.subscribers -= OnZoomPressed;
			}
		}

		protected override void Add(MachineInvisibilityNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				node.machineVisibilityComponent.machineBecameInvisible.subscribers += HandleCloakActivated;
				node.machineVisibilityComponent.machineBecameVisible.subscribers += HandleCloakDeactivated;
			}
		}

		protected override void Remove(MachineInvisibilityNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				TryCancelZoom();
				node.machineVisibilityComponent.machineBecameInvisible.subscribers -= HandleCloakActivated;
				node.machineVisibilityComponent.machineBecameVisible.subscribers -= HandleCloakDeactivated;
			}
		}

		protected override void Add(CameraDeathAnimationEntityView entityView)
		{
			entityView.deathAnimationBroadcastComponent.isAnimating.NotifyOnValueSet((Action<int, bool>)OnDeathAnimationStateChanged);
		}

		protected override void Remove(CameraDeathAnimationEntityView entityView)
		{
			entityView.deathAnimationBroadcastComponent.isAnimating.StopNotify((Action<int, bool>)OnDeathAnimationStateChanged);
		}

		void IInitialize.OnDependenciesInjected()
		{
			switchWeaponObserver.SwitchWeaponsEvent += HandleOnWeaponSwitched;
			spawnDispatcher.OnPlayerRegistered += OnPlayerRegistered;
			spectatorModeActivator.Register(OnSpectatorMode);
			gameEndedObserver.OnGameEnded += HandleOnGameEnded;
			_toggleZoom = mouseSettings.IsToggleZoom();
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			switchWeaponObserver.SwitchWeaponsEvent -= HandleOnWeaponSwitched;
			spawnDispatcher.OnPlayerRegistered -= OnPlayerRegistered;
			spectatorModeActivator.Unregister(OnSpectatorMode);
			gameEndedObserver.OnGameEnded -= HandleOnGameEnded;
		}

		private void HandleOnGameEnded(bool gameWon)
		{
			TryCancelZoom();
		}

		private void TryCancelZoom()
		{
			if (_zoom == ZoomType.Zoomed)
			{
				_zoom = ZoomType.NoZoom;
				SetPlayerVisibility();
				SetCameraFoV();
				this.OnZoomModeChange(_zoom, _zoomAmount);
			}
		}

		private void HandleCloakDeactivated(IMachineVisibilityComponent sender, int machineId)
		{
			_isPlayerCloaked = false;
		}

		private void HandleCloakActivated(IMachineVisibilityComponent sender, int machineId)
		{
			_isPlayerCloaked = true;
		}

		private void OnSpectatorMode(int killer, bool enabled)
		{
			_isPlayerSpectator = enabled;
		}

		private void OnZoomPressed(int machineId)
		{
			if (machineId != _machineId)
			{
				return;
			}
			bool flag = false;
			if (!_isPlayerSpectator)
			{
				IMachineInputComponent machineInput = entityViewsDB.QueryEntityView<MachineInputNode>(machineId).machineInput;
				flag = DoWeWantToZoom(machineInput.fire2);
			}
			ZoomType zoom = _zoom;
			if (flag)
			{
				if (_canZoom)
				{
					_zoom = ZoomType.Zoomed;
				}
			}
			else
			{
				_zoom = ZoomType.NoZoom;
			}
			if (zoom != _zoom)
			{
				SetPlayerVisibility();
				SetCameraFoV();
				PlayZoomAudio();
				this.OnZoomModeChange(_zoom, _zoomAmount);
			}
		}

		private bool DoWeWantToZoom(float zoomValue)
		{
			bool flag = zoomValue > 0f;
			if (_toggleZoom)
			{
				flag = ((_lastInput || !flag) ? (_zoom == ZoomType.Zoomed) : (_zoom == ZoomType.NoZoom));
			}
			_lastInput = (zoomValue > 0f);
			return flag;
		}

		private void SetPlayerVisibility()
		{
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			bool enabled = !zoomed;
			if (_playerFlagTrails != null)
			{
				for (int i = 0; i < _playerFlagTrails.Length; i++)
				{
					XWeaponTrail xWeaponTrail = _playerFlagTrails[i];
					_playerRobotRenderers.Add(xWeaponTrail.MMeshObj.GetComponent<Renderer>());
				}
				_playerFlagTrails = null;
			}
			for (int j = 0; j < _playerRobotRenderers.get_Count(); j++)
			{
				if (_playerRobotRenderers.get_Item(j) != null)
				{
					if (zoomed || !_isPlayerCloaked || (_isPlayerCloaked && !(_playerRobotRenderers.get_Item(j) is ParticleSystemRenderer) && !(_playerRobotRenderers.get_Item(j) is TrailRenderer)))
					{
						_playerRobotRenderers.get_Item(j).set_enabled(enabled);
					}
				}
				else
				{
					_playerRobotRenderers.UnorderedRemoveAt(j--);
				}
			}
			for (int k = 0; k < _playerRobotWidgets.get_Count(); k++)
			{
				if (_playerRobotWidgets.get_Item(k) != null)
				{
					_playerRobotWidgets.get_Item(k).set_enabled(enabled);
				}
				else
				{
					_playerRobotWidgets.UnorderedRemoveAt(k--);
				}
			}
			if (_playerRobotPitLeaderEffects != null)
			{
				EmissionModule emission = _playerRobotPitLeaderEffects.leaderEffectsParticleSystem.get_emission();
				emission.set_enabled(enabled);
			}
			foreach (KeyValuePair<int, ZoomNode> allWeapon in _allWeapons)
			{
				allWeapon.Value.zoomComponent.isZoomed = zoomed;
			}
		}

		private void SetCameraFoV()
		{
			float num = _defaultFoV;
			if (zoomed)
			{
				num = _zoomedFoV;
			}
			for (int i = 0; i < _cameras.Length; i++)
			{
				_cameras[i].set_fieldOfView(num);
			}
			_zoomAmount = _defaultFoV / num;
			sqrDrawDistanceScale = _zoomAmount;
			sqrDrawDistanceScale *= sqrDrawDistanceScale;
		}

		private void PlayZoomAudio()
		{
			AudioFabricGameEvents eventEnum = (_zoom != ZoomType.Zoomed) ? AudioFabricGameEvents.WeaponZoomOut : AudioFabricGameEvents.WeaponZoomIn;
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(eventEnum), 0);
		}

		private void OnPlayerRegistered(SpawnInParametersPlayer data)
		{
			if (data.isMe)
			{
				_playerRobotRenderers = data.preloadedMachine.allRenderers;
				FindRenderers(data.preloadedMachine.machineBoard);
			}
		}

		private void FindRenderers(GameObject machineRoot)
		{
			EnsureWeHaveCameras();
			_playerRobotWidgets = new FasterList<UIWidget>((ICollection<UIWidget>)machineRoot.GetComponentsInChildren<UIWidget>(true));
			_playerRobotPitLeaderEffects = machineRoot.GetComponentInChildren<PitLeaderFX>();
			if (machineRoot.GetComponentsInChildren<XWeaponTrail>().Length > 0)
			{
				_playerFlagTrails = machineRoot.GetComponentsInChildren<XWeaponTrail>();
			}
		}

		private void EnsureWeHaveCameras()
		{
			if (_cameras == null)
			{
				_cameras = Camera.get_main().get_gameObject().GetComponentsInChildren<Camera>(true);
				_defaultFoV = _cameras[0].get_fieldOfView();
				for (int i = 1; i < _cameras.Length; i++)
				{
				}
			}
		}

		private void HandleOnWeaponSwitched(int machineId, ItemDescriptor subcategory)
		{
			_currentActiveSubcategory = subcategory;
			if (_weaponCategoryZoomMode.ContainsKey(subcategory))
			{
				float zoomedFoV = _zoomedFoV;
				_canZoom = _weaponCategoryZoomMode[subcategory].zoomComponent.canZoom;
				_zoomedFoV = _weaponCategoryZoomMode[subcategory].zoomComponent.zoomedFov;
				if (_zoom == ZoomType.Zoomed && zoomedFoV != _zoomedFoV && _canZoom)
				{
					SetCameraFoV();
					this.OnZoomModeChange(_zoom, _zoomAmount);
					PlayZoomAudio();
				}
			}
			else
			{
				_canZoom = false;
			}
		}

		private void OnDeathAnimationStateChanged(int entityId, bool state)
		{
			TryCancelZoom();
			_isPlayerSpectator = state;
		}
	}
}
