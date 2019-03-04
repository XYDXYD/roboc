using Simulation;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class MinimapView : MonoBehaviour, IInitialize, IMiniMapViewComponent, IComponent
{
	private const float REDUCED_ZOOM_PERCENTAGE = 0.66f;

	private const int MAX_MAP_SPRITES = 50;

	public UIPanel panel;

	public UITexture mapTexture;

	public UISprite playerSprite;

	public GameObject player;

	public UISprite blueBaseSprite;

	public UISprite redBaseSprite;

	public UISprite cameraSprite;

	public UISprite[] towerSprites;

	public UISprite[] towerInnerSprites;

	public UISprite equalizerSprite;

	public UISprite equalizerBottomSprite;

	public UIAnchor anchor;

	public Color friendlyTowerColour = new Color(2f / 51f, 163f / 255f, 0.9411765f);

	public Color enemyTowerColour = new Color(0.7058824f, 0f, 0f);

	public Color friendlyTowerInnerColour = new Color(2f / 51f, 163f / 255f, 0.9411765f);

	public Color enemyTowerInnerColour = new Color(0.7058824f, 0f, 0f);

	public float closeTime;

	public GameObject pingSelector;

	public GameObject[] pingIndicators;

	public Transform equalizerTransform;

	public Texture2D defaultMouseCursor;

	public Texture2D pingMouseCursor;

	private int _lastKnownResolutionY;

	private List<UISprite> _sprites = new List<UISprite>();

	private Dictionary<int, int> _spritesBindings = new Dictionary<int, int>();

	private Stack<int> _availableIndices = new Stack<int>();

	private Vector2 _pixelOffset = Vector2.get_zero();

	private float _zoomedScaleFactor;

	private float _reducedScaleFactor;

	private bool _isMinimapZoomed;

	private bool _isPingSystemActive;

	private bool _canPing;

	[Inject]
	internal IMinimapPresenter presenter
	{
		private get;
		set;
	}

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	[Inject]
	internal LobbyGameStartPresenter lobbyGameStartPesenter
	{
		private get;
		set;
	}

	[Inject]
	internal ChatPresenter chatPresenter
	{
		private get;
		set;
	}

	public MinimapView()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	//IL_002a: Unknown result type (might be due to invalid IL or missing references)
	//IL_002f: Unknown result type (might be due to invalid IL or missing references)
	//IL_0044: Unknown result type (might be due to invalid IL or missing references)
	//IL_0049: Unknown result type (might be due to invalid IL or missing references)
	//IL_005e: Unknown result type (might be due to invalid IL or missing references)
	//IL_0063: Unknown result type (might be due to invalid IL or missing references)
	//IL_008a: Unknown result type (might be due to invalid IL or missing references)
	//IL_008f: Unknown result type (might be due to invalid IL or missing references)


	void IInitialize.OnDependenciesInjected()
	{
		presenter.RegisterView(this);
		LobbyGameStartPresenter lobbyGameStartPesenter = this.lobbyGameStartPesenter;
		lobbyGameStartPesenter.OnInitialLobbyGuiClose = (Action)Delegate.Combine(lobbyGameStartPesenter.OnInitialLobbyGuiClose, new Action(OnInitialGuiDisabled));
		for (int i = 0; i < pingIndicators.Length; i++)
		{
			gameObjectFactory.RegisterPrefab(pingIndicators[i], pingIndicators[i].get_name(), this.get_transform().GetChild(0).get_gameObject());
		}
	}

	private void OnDestroy()
	{
		presenter.OnDestroy();
	}

	public void SetEqualizerPosition(Vector2 position)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f - position.x;
		Vector2 localSize = mapTexture.get_localSize();
		float num2 = num * localSize.x;
		float y = position.y;
		Vector2 localSize2 = mapTexture.get_localSize();
		Vector3 localPosition = default(Vector3);
		localPosition._002Ector(num2, y * localSize2.y, 0f);
		equalizerTransform.set_localPosition(localPosition);
		equalizerTransform.get_gameObject().SetActive(false);
	}

	public void SetBasePosition(Vector2 position, bool isFriendly)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f - position.x;
		Vector2 localSize = mapTexture.get_localSize();
		float num2 = num * localSize.x;
		float y = position.y;
		Vector2 localSize2 = mapTexture.get_localSize();
		Vector3 localPosition = default(Vector3);
		localPosition._002Ector(num2, y * localSize2.y, 0f);
		if (isFriendly)
		{
			blueBaseSprite.get_transform().set_localPosition(localPosition);
			blueBaseSprite.get_gameObject().SetActive(true);
		}
		else
		{
			redBaseSprite.get_transform().set_localPosition(localPosition);
			redBaseSprite.get_gameObject().SetActive(true);
		}
	}

	public void SetCapturePointPosition(Vector2 position, int towerIndex)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		UISprite val = towerSprites[towerIndex];
		Transform transform = val.get_transform();
		float num = 0f - position.x;
		Vector2 localSize = mapTexture.get_localSize();
		float num2 = num * localSize.x;
		float y = position.y;
		Vector2 localSize2 = mapTexture.get_localSize();
		transform.set_localPosition(new Vector3(num2, y * localSize2.y, 0f));
		val.get_gameObject().SetActive(true);
	}

	public void SetCapturePointTeam(bool isMyTeam, int towerIndex)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		UISprite val = towerSprites[towerIndex];
		UISprite val2 = towerInnerSprites[towerIndex];
		if (isMyTeam)
		{
			val.set_color(friendlyTowerColour);
			val2.set_color(friendlyTowerInnerColour);
		}
		else
		{
			val.set_color(enemyTowerColour);
			val2.set_color(enemyTowerInnerColour);
		}
	}

	public void SetEqualizerOwner(bool isMyTeam)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (isMyTeam)
		{
			equalizerBottomSprite.set_color(friendlyTowerColour);
			equalizerSprite.set_color(friendlyTowerColour);
		}
		else
		{
			equalizerBottomSprite.set_color(enemyTowerColour);
			equalizerSprite.set_color(enemyTowerColour);
		}
		equalizerTransform.get_gameObject().SetActive(true);
	}

	public void HideEqualizer()
	{
		equalizerTransform.get_gameObject().SetActive(false);
	}

	public void SetTexture(Texture texture)
	{
		mapTexture.set_mainTexture(texture);
	}

	public void UpdateSprite(int playerId, Vector2 position, Quaternion rotation)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		int index = _spritesBindings[playerId];
		UISprite val = _sprites[index];
		float x = position.x;
		Vector2 localSize = mapTexture.get_localSize();
		position.x = x * (0f - localSize.x);
		float y = position.y;
		Vector2 localSize2 = mapTexture.get_localSize();
		position.y = y * localSize2.y;
		val.get_transform().set_localPosition(new Vector3(position.x, position.y, 0f));
		val.get_transform().set_localRotation(rotation);
	}

	public void UpdatePlayerSprite(Vector2 position, Quaternion orientation, Quaternion cameraOrientation)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		float x = position.x;
		Vector2 localSize = mapTexture.get_localSize();
		position.x = x * (0f - localSize.x);
		float y = position.y;
		Vector2 localSize2 = mapTexture.get_localSize();
		position.y = y * localSize2.y;
		playerSprite.get_transform().set_localPosition(new Vector3(position.x, position.y, 0f));
		playerSprite.get_transform().set_localRotation(orientation);
		cameraSprite.get_transform().set_localRotation(cameraOrientation);
		cameraSprite.get_transform().set_localPosition(new Vector3(position.x, position.y, 0f));
	}

	private void Start()
	{
		Initialize();
		presenter.Start();
		CalculateAllSizeAdjustments();
	}

	private void CalculateAllSizeAdjustments()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)Screen.get_height() * 0.5f - anchor.pixelOffset.y;
		float num2 = num;
		Vector2 localSize = mapTexture.get_localSize();
		_zoomedScaleFactor = num2 / localSize.y;
		_reducedScaleFactor = _zoomedScaleFactor * 0.66f;
		_pixelOffset = this.GetComponent<UIAnchor>().pixelOffset;
		panel.get_transform().set_localScale(Vector3.get_one() * _reducedScaleFactor);
		IMinimapPresenter presenter = this.presenter;
		Vector2 localSize2 = mapTexture.get_localSize();
		presenter.MinimapResized(localSize2.y * _reducedScaleFactor + anchor.pixelOffset.y);
		_lastKnownResolutionY = Screen.get_height();
	}

	private void Update()
	{
		if (Screen.get_height() != _lastKnownResolutionY)
		{
			CalculateAllSizeAdjustments();
		}
		if (lobbyGameStartPesenter.hasBeenClosed && InputRemapper.Instance.GetButtonDown("MinimapZoom") && !_isPingSystemActive && (chatPresenter == null || !chatPresenter.IsChatFocused()))
		{
			OnToggleMinimap();
		}
	}

	private void OnToggleMinimap()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		_isMinimapZoomed = !_isMinimapZoomed;
		float num = (!_isMinimapZoomed) ? _reducedScaleFactor : _zoomedScaleFactor;
		panel.get_transform().set_localScale(Vector3.get_one() * num);
		IMinimapPresenter presenter = this.presenter;
		Vector2 localSize = mapTexture.get_localSize();
		presenter.MinimapResized(localSize.y * num + anchor.pixelOffset.y);
	}

	public void OnInitialGuiDisabled()
	{
		if (lobbyGameStartPesenter.hasBeenClosed)
		{
			this.get_gameObject().SetActive(true);
		}
	}

	private void Initialize()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		playerSprite.get_gameObject().SetActive(false);
		cameraSprite.get_gameObject().SetActive(false);
		for (int i = 0; i < 50; i++)
		{
			GameObject val = gameObjectFactory.Build(player);
			val.get_transform().set_parent(panel.get_transform());
			val.get_transform().set_localScale(Vector3.get_one());
			val.get_transform().set_localPosition(Vector3.get_zero());
			UISprite component = val.GetComponent<UISprite>();
			val.SetActive(false);
			_sprites.Add(component);
			_availableIndices.Push(i);
		}
		this.get_gameObject().SetActive(false);
	}

	public void Enable(int playerId, bool enabled)
	{
		int index = _spritesBindings[playerId];
		_sprites[index].get_gameObject().SetActive(enabled);
	}

	public void RegisterMainPlayer()
	{
		playerSprite.get_gameObject().SetActive(true);
		cameraSprite.get_gameObject().SetActive(true);
	}

	public void UnregisterMainPlayer()
	{
		playerSprite.get_gameObject().SetActive(false);
		cameraSprite.get_gameObject().SetActive(false);
	}

	public void setPlayerColor(Color color)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		playerSprite.set_color(color);
	}

	public void RegisterPlayer(int playerId, bool isOnMyTeam, bool isInMyPlatoon)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		int num = _availableIndices.Pop();
		_spritesBindings[playerId] = num;
		if (isOnMyTeam)
		{
			_sprites[num].set_color(PlayerColours.playerColor);
			if (isInMyPlatoon)
			{
				_sprites[num].set_color(PlayerColours.platoonColor);
			}
		}
	}

	public void PingSpottedPlayer(int playerId)
	{
		_sprites[_spritesBindings[playerId]].GetComponent<Animation>().Play("HUD_Minimap_Enemy_Spotted");
	}

	public void UnregisterPlayer(int playerId)
	{
		int num = _spritesBindings[playerId];
		_availableIndices.Push(num);
		_spritesBindings.Remove(playerId);
		_sprites[num].get_gameObject().SetActive(false);
	}

	public float GetZoomedMapFactor()
	{
		return _zoomedScaleFactor;
	}

	public bool GetIsMinimapZoomed()
	{
		return _isMinimapZoomed;
	}

	public void ToggleMinimap()
	{
		OnToggleMinimap();
	}

	public bool GetIsPingContextActive()
	{
		return _isPingSystemActive;
	}

	public float GetCloseTime()
	{
		return closeTime;
	}

	public float GetScaledHalfMapSize()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 localSize = mapTexture.get_localSize();
		return localSize.x * _zoomedScaleFactor / 2f;
	}

	public float GetUnscaledHalfMapSize()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 localSize = mapTexture.get_localSize();
		return localSize.x / 2f;
	}

	public Vector2 GetPixelOffset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return _pixelOffset;
	}

	public void SetIsPingContextActive(bool active)
	{
		_isPingSystemActive = active;
	}

	public bool GetCanPing()
	{
		return _canPing;
	}

	public void SetCanPing(bool canPing)
	{
		_canPing = canPing;
	}

	public string GetPingIndicatorNameOfType(PingType type)
	{
		return pingIndicators[(int)type].get_name();
	}

	public Texture2D GetDefaultMouseCursor()
	{
		return defaultMouseCursor;
	}

	public Texture2D GetPingMouseCursor()
	{
		return pingMouseCursor;
	}
}
