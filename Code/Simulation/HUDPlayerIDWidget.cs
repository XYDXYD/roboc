using Battle;
using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class HUDPlayerIDWidget : MonoBehaviour
	{
		public UILabel nameLabel;

		public UILabel healthPercentLabel;

		public UISprite spotIcon;

		public float alphaFadeInSpeed = 10f;

		public float alphaFadeOutSpeed = 0.5f;

		public float fadeAtRange = 10f;

		public float fogStartDistance = 200f;

		public float distanceOffsetScaler = 0.01f;

		public UIPanel weaponIconsPanel;

		public UISlider healthBarSlider;

		public UISlider healthBarUpdateSlider;

		public UISprite healthBar;

		public UIWidget nameAndHealthWidget;

		public GameObject leaverBuffGO;

		public float healthBarUpdateRate = 0.05f;

		public Vector3 screenPosOffset = new Vector3(0f, 12f, 0f);

		public Vector3 worldPosOffset = new Vector3(0f, 0.6f, 0f);

		public Color friendlyColor = Color.get_blue();

		public Color enemyColor = Color.get_red();

		public Color platoonColor = Color.get_green();

		public PlayerMarkerConfig playerMarkerConfig;

		public UITexture AvatarTexture;

		public HUDPlayerClanAvatarIDWidget clanAndPlayerAvatarHolder;

		public Transform floatingNumbersHolder;

		private Transform _transform;

		private Transform _myTransform;

		private PreloadedMachine _preloadedMachine;

		private Camera _mainCamera;

		private Transform _mainCameraTransform;

		private int _currentRayCount;

		private bool _machineIsVisible;

		private int _layerMask;

		private Vector3 _yOffset = Vector3.get_zero();

		private Vector3 _centerOfMass;

		private float _cameraMachineDistance = 1f;

		private bool _infrontOfCamera;

		private bool _playerHit;

		private float _playerHitTimer = 2f;

		private int _ownerID = -1;

		private bool _isPlatoon;

		private bool _isAlly;

		private float _currentAlpha;

		private float _YOffset;

		private bool _YOffsetDirty = true;

		private static int _staticRayCount;

		private const int CAST_RAY_EVERY_N_FRAMES = 30;

		private HUDPlayerIdWidgetExtension[] _extensions;

		[Inject]
		internal NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer playerNamesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal IMinimapPresenter minimapPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal BattlePlayers battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		internal MachinePreloader machinePreloader
		{
			private get;
			set;
		}

		public HUDPlayerIDWidget()
			: this()
		{
		}//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)


		private void Awake()
		{
			_myTransform = this.get_transform();
		}

		private void Start()
		{
			_currentRayCount = _staticRayCount;
			_staticRayCount++;
			_layerMask = ((1 << GameLayers.TERRAIN) | (1 << GameLayers.PROPS));
			_mainCamera = Camera.get_main();
			_mainCameraTransform = _mainCamera.get_transform();
			_transform = this.get_transform();
			_extensions = this.GetComponents<HUDPlayerIdWidgetExtension>();
		}

		private void OnEnable()
		{
			nameAndHealthWidget.set_alpha(0f);
			_currentAlpha = 0f;
		}

		private void LateUpdate()
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			float value = healthBarSlider.get_value();
			if (healthBarUpdateSlider.get_value() > value)
			{
				healthBarUpdateSlider.set_value(Mathf.Max(value, healthBarUpdateSlider.get_value() - healthBarUpdateRate * Time.get_deltaTime()));
			}
			else if (healthBarUpdateSlider.get_value() < value)
			{
				healthBarUpdateSlider.set_value(value);
			}
			if (_preloadedMachine != null)
			{
				_centerOfMass = _preloadedMachine.machineInfo.centerTransform.get_position();
				_cameraMachineDistance = Vector3.Distance(_mainCameraTransform.get_position(), _centerOfMass);
				_infrontOfCamera = (Vector3.Dot(_mainCameraTransform.get_forward(), _mainCameraTransform.get_position() - _centerOfMass) < 0f);
				if (!_infrontOfCamera)
				{
					_currentAlpha = 0f;
				}
				_myTransform.set_localPosition(CalculateHUDPosition());
				UpdateScale();
				UpdateHitTimer();
				bool flag = minimapPresenter.GetVisible(_ownerID);
				if (WorldSwitching.GetGameModeType() == GameModeType.Campaign)
				{
					flag = false;
				}
				bool flag2 = !_isAlly && flag;
				if (!IsMachineInEnemyDetectRange())
				{
					_machineIsVisible = false;
				}
				bool render = _playerHit || _isAlly || (IsMachineInEnemyDetectRange() && IsMachineVisible());
				if (flag2 && IsMachineInCameraClippingRange())
				{
					render = true;
				}
				UpdateAlpha(render);
				UpdateExtensions();
			}
		}

		public void InitHUDPlayerIDWidget(int machineId, string playerName, string displayName, uint currentHealth, uint totalHealth)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			_mainCamera = Camera.get_main();
			_mainCameraTransform = _mainCamera.get_transform();
			_transform = this.get_transform();
			nameLabel.set_text(displayName);
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, machineId);
			_preloadedMachine = machinePreloader.GetPreloadedMachine(playerName);
			_yOffset.y = CalculateYOffset(machineId, _preloadedMachine.machineInfo.centerTransform.get_localPosition());
			_ownerID = playerFromMachineId;
			healthBarSlider.set_value(1f);
			healthBarUpdateSlider.set_value(healthBarSlider.get_value());
			InitialiseColors(playerFromMachineId, playerName);
		}

		public void ResetHealth()
		{
			healthBarSlider.set_value(1f);
			healthBarUpdateSlider.set_value(healthBarSlider.get_value());
		}

		public void HealthChange(float percent, bool shooterIsMe)
		{
			if (shooterIsMe)
			{
				_playerHit = true;
				_playerHitTimer = 2f;
			}
			_YOffsetDirty = true;
			UpdateHealth(percent);
		}

		public void TogglePlayerBuffed(bool isBuffed)
		{
			if (leaverBuffGO != null)
			{
				leaverBuffGO.SetActive(isBuffed);
			}
		}

		private void InitialiseColors(int playerId, string name)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerId))
			{
				int myPlatoonId = battlePlayers.MyPlatoonId;
				int platoonId = battlePlayers.GetPlatoonId(name);
				if (myPlatoonId != 255 && myPlatoonId == platoonId)
				{
					SetColors(platoonColor);
					_isPlatoon = true;
				}
				else
				{
					SetColors(friendlyColor);
					_isAlly = true;
				}
			}
			else
			{
				SetColors(enemyColor);
			}
		}

		private void UpdateAlpha(bool render)
		{
			_currentAlpha = Mathf.Clamp01(_currentAlpha + CalculateAlphaChange(render | _isPlatoon));
			nameAndHealthWidget.set_alpha(_currentAlpha);
			nameAndHealthWidget.set_enabled(_currentAlpha > 0f);
		}

		private float CalculateAlphaChange(bool show)
		{
			float num = (!show) ? (0f - alphaFadeOutSpeed) : alphaFadeInSpeed;
			return num * Time.get_deltaTime();
		}

		private void UpdateHitTimer()
		{
			if (_playerHit)
			{
				_playerHitTimer -= Time.get_deltaTime();
				if (_playerHitTimer <= 0f)
				{
					_playerHit = false;
				}
			}
		}

		private void UpdateScale()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			float num = _cameraMachineDistance - playerMarkerConfig.minDistance;
			float num2 = 1f - Mathf.Clamp01(num / (playerMarkerConfig.maxDistance - playerMarkerConfig.minDistance));
			Vector3 localScale = Vector3.get_one() * (playerMarkerConfig.maxDistanceScale + num2 * (playerMarkerConfig.minDistanceScale - playerMarkerConfig.maxDistanceScale));
			_transform.set_localScale(localScale);
		}

		private void UpdateExtensions()
		{
			for (int i = 0; i < _extensions.Length; i++)
			{
				_extensions[i].UpdateAlpha(_currentAlpha);
			}
		}

		private void UpdateHealth(float percent)
		{
			healthBarSlider.set_value(percent);
		}

		private void OnDisable()
		{
			_playerHit = false;
		}

		private void SetColors(Color color)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			nameLabel.set_color(color);
			if (spotIcon != null)
			{
				spotIcon.set_color(color);
			}
			healthBar.set_color(color);
			if (leaverBuffGO != null)
			{
				UISprite[] componentsInChildren = leaverBuffGO.GetComponentsInChildren<UISprite>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].set_color(color);
				}
			}
		}

		private bool IsScreenPosOnScreen()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = _mainCamera.WorldToViewportPoint(_centerOfMass);
			if ((_infrontOfCamera && val.x > 0f && val.x < 1f && val.y > 0f) || val.y < 1f)
			{
				return true;
			}
			_machineIsVisible = false;
			_currentAlpha = 0f;
			return false;
		}

		private bool IsMachineInEnemyDetectRange()
		{
			if (_cameraMachineDistance < fadeAtRange)
			{
				return true;
			}
			return false;
		}

		private bool IsMachineInCameraClippingRange()
		{
			if (_cameraMachineDistance < fogStartDistance)
			{
				return true;
			}
			return false;
		}

		private bool IsMachineVisible()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			_currentRayCount++;
			if (_currentRayCount > 30)
			{
				Vector3 val = _preloadedMachine.machineInfo.centerTransform.get_position() - _mainCameraTransform.get_position();
				Ray val2 = default(Ray);
				val2._002Ector(_mainCameraTransform.get_position(), val);
				_currentRayCount = 0;
				_machineIsVisible = !Physics.Raycast(val2, _cameraMachineDistance, _layerMask);
			}
			return _machineIsVisible;
		}

		private float CalculateYOffset(int machineId, Vector3 centre)
		{
			if (_YOffsetDirty)
			{
				Vector3[] minAndMaxCubePos = GridScaleUtility.GetMinAndMaxCubePos((IEnumerable<InstantiatedCube>)machineManager.GetMachineMap(TargetType.Player, machineId).GetAllInstantiatedCubes(), TargetType.Player);
				_YOffset = minAndMaxCubePos[1].y;
			}
			return _YOffset - centre.y;
		}

		private Vector3 CalculateHUDPosition()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return GuiUtil.CalculateHUDPosition(_centerOfMass + _yOffset + worldPosOffset, screenPosOffset, this.get_transform(), _infrontOfCamera);
		}
	}
}
